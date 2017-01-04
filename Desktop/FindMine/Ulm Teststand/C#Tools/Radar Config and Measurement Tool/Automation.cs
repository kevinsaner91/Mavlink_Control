using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using EH.RadarControl;

namespace Radar_Config_and_Measurement_Tool
{
    public partial class Main : Form
    {
        private StreamWriter sw;
        private bool automationCancelationPending = false;

        protected void printDebugMessage(string text)
        {
            if (enableAutomationDebugOutputToolStripMenuItem.Checked)
            {
                using (StreamWriter sw = new StreamWriter("automation.dbg", true))
                {
                    sw.WriteLine(DateTime.Now.ToString() + " - " + text);
                }
            }
        }

        private void btn_openAutomationScript_Click(object sender, EventArgs e)
        {
            if (openFileDialogAutomation.ShowDialog() == DialogResult.OK)
            {
                tB_AutomationScriptPath.Text = openFileDialogAutomation.FileName;
            }
        }

        private void btn_Automation_browseLogFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog diag = new SaveFileDialog();
            diag.Filter = "log files (*.log)|*.log";
            diag.FilterIndex = 1;
            diag.RestoreDirectory = true;

            if (diag.ShowDialog() == DialogResult.OK)
            {
                tB_Automation_LogFile.Text = diag.FileName;
            }
        }

        private void cB_Automation_saveLogToFile_CheckedChanged(object sender, EventArgs e)
        {
            panel_Automation_Logfile.Enabled = cB_Automation_saveLogToFile.Checked;
        }

        private void appendToAutomationLog(string text)
        {
            rtb_AutomationLog.AppendText(text);
            if (cB_Automation_saveLogToFile.Checked)
            {
                sw.Write(text);
                sw.Flush();
            }
        }

        private void finalizeAutomation(PositionControl control=null, PositionTracker tracker=null)
        {
            if (cB_Automation_saveLogToFile.Checked && sw != null)
            {
                sw.Close();
            }

            if (control != null)
                control.closeCOM();

            if (tracker != null)
                tracker.closeCOM();

            pnl_measMultiple.Enabled = true;
            btn_startAutomation.Visible = true;
            btn_Capture_StartStop.Enabled = true;
            btn_cancelAutomation.Visible = false;
            btn_cancelAutomation.Enabled = true;
            btn_cancelAutomation.Text = "cancel";
        }

        private void btn_cancelAutomation_Click(object sender, EventArgs e)
        {
            automationCancelationPending = true;
            btn_cancelAutomation.Enabled = false;
            btn_cancelAutomation.Text = "cancelation pending...";
        }

        private void btn_startAutomation_Click(object sender, EventArgs e)
        {       
            if (!File.Exists(tB_AutomationScriptPath.Text))
            {
                MessageBox.Show("The given script file does not exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            rtb_AutomationLog.Clear();
            if (cB_Automation_saveLogToFile.Checked && tB_Automation_LogFile.Text.Length > 3)
            {
                if (tB_Automation_LogFile.Text.Length < 3)
                {
                    cB_Automation_saveLogToFile.Checked = false;
                }
                else
                {
                    sw = new StreamWriter(tB_Automation_LogFile.Text, false); //do not append create new file
                }
            }
            pnl_measMultiple.Enabled = false;

            appendToAutomationLog("Automation started: " + DateTime.Now.ToString() + "\n");
            printDebugMessage("Automation started");

            //Analyzing Script file
            appendToAutomationLog("\nAnalyzing Script...");

            SettingsCollector.Track track = SettingsCollector.Track.MillimeterTrack;
            SettingsCollector.Board module = SettingsCollector.Board.TankGauging;
            UInt16 positionControlCOM = 1;
            UInt16 positionTrackerCOM = 2;
            UInt16 preCaptures = 0;
            double isel_stepsize = 0;
            UInt32 isel_maxDist = 0;
            bool performReferenceDrive = false;

            List<st_Automation_Task> tasks = new List<st_Automation_Task>();

            XmlReader reader = XmlReader.Create(tB_AutomationScriptPath.Text);
            bool startFound = false;
            while (reader.Read())
            {
                try
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "RadarAutomation")
                    {
                        startFound = true;
                        if (Convert.ToUInt32(reader["ScriptVersion"]) != 1)
                        {
                            appendToAutomationLog("Invalid script version!\n");
                            reader.Close();
                            finalizeAutomation();
                            return;
                        }

                        while (reader.NodeType != XmlNodeType.EndElement)
                        {
                            reader.Read();

                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "BasicSetup")
                            {
                                track = (SettingsCollector.Track)Convert.ToUInt32(reader["TrackID"]);
                                module = (SettingsCollector.Board)Convert.ToUInt32(reader["ModuleID"]);
                                positionControlCOM = Convert.ToUInt16(reader["PositionControlCOM"]);
                                positionTrackerCOM = Convert.ToUInt16(reader["PositionTrackerCOM"]);
                                performReferenceDrive = Convert.ToBoolean(reader["PerformRefDrive"]);
                                preCaptures = Convert.ToUInt16(reader["PreCaptures"]);

                                if (track == SettingsCollector.Track.Uni_Ulm)
                                {
                                    isel_stepsize = Convert.ToDouble(reader["iselIMC4Steps"]);
                                    isel_maxDist = Convert.ToUInt16(reader["iselIMC4MaxDist"]);
                                }
                            }

                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Tasks")
                            {
                                while (reader.NodeType != XmlNodeType.EndElement)
                                {
                                    reader.Read();
                                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Task")
                                    {
                                        st_Automation_Task task;

                                        task.outputFolder = reader["OutputFolder"];
                                        task.configFile = reader["ConfigFile"];
                                        task.startDistance = Convert.ToUInt32(reader["StartDistance"]);
                                        task.stopDistance = Convert.ToUInt32(reader["StopDistance"]);
                                        task.steps = Convert.ToUInt32(reader["Steps"]);
                                        task.multidataSteps = Convert.ToUInt32(reader["MultidataSteps"]);
                                        task.multidataAmount = Convert.ToUInt32(reader["MultidataAmount"]);

                                        if (task.configFile.Length > 1 && !File.Exists(task.configFile))
                                        {
                                            appendToAutomationLog("failed!\nConfig File " + task.configFile + " doesn't exist!\n");
                                            reader.Close();
                                            finalizeAutomation();
                                            return;
                                        }

                                        tasks.Add(task);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    appendToAutomationLog("Script invalid!\n");
                    finalizeAutomation();
                    return;
                }
            }
            if (!startFound || tasks.Count == 0)
            {
                appendToAutomationLog("Script invalid!\n");
                finalizeAutomation();
                return;
            }
            reader.Close();
            appendToAutomationLog("done\n");

            //update Tabs
            SettingsCollector.BoardSelection = module;
            SettingsCollector.TrackSelection = track;
            updateTabSelection(sender, e);
            tC_Main.SelectedTab = tP_Automation;

            appendToAutomationLog("\nLoaded " + tasks.Count + " tasks.\n");
            printDebugMessage("Loaded " + tasks.Count + " tasks.");

            this.Refresh();

            bool debugEnabled = enableAutomationDebugOutputToolStripMenuItem.Checked;

            PositionControl control=null;
            PositionTracker tracker=null;
            switch (track)
            {
                case SettingsCollector.Track.MillimeterTrack:
                    tracker = new MillTrack_PositionTracker(debugEnabled);
                    control = new MillTrack_PositionControl(debugEnabled);
                    break;
                case SettingsCollector.Track.FEW_Lab:
                    tracker = new Lab_PositionTracker(debugEnabled);
                    control = new Lab_PositionControl(debugEnabled);
                    break;
                case SettingsCollector.Track.Uni_Ulm:
                    control = new UniUlm_PositionControl(isel_stepsize, isel_maxDist, debugEnabled);
                    tracker = new UniUlm_PositionTracker(control as UniUlm_PositionControl, debugEnabled);
                    break;
                case SettingsCollector.Track.SimulatorTrack:
                    tracker = new Simulator_PositionTracker(debugEnabled);
                    control = new Simulator_PositionControl(debugEnabled);
                    break;
                default:
                    appendToAutomationLog("Not a valid track selected. Requested Track ID: " + (int)track + "\n");
                    finalizeAutomation();
                    break;
            }

            if (track != SettingsCollector.Track.Uni_Ulm)
            {
                appendToAutomationLog("\nOpen Position Tracker: COM" + positionTrackerCOM + ": ");
                if (!tracker.openCOM("COM" + positionTrackerCOM))
                {
                    appendToAutomationLog("failed!\n");
                    finalizeAutomation();
                    return;
                }
                appendToAutomationLog("successful");
            }

            appendToAutomationLog("\nOpen Position Control: COM" + positionControlCOM + ": ");
            if (!control.openCOM("COM" + positionControlCOM))
            {
                appendToAutomationLog("failed!\n");
                finalizeAutomation(null, tracker);
                return;
            }
            appendToAutomationLog("successful\n");
            
            //everything loaded successfully
            automationCancelationPending = false;
            btn_startAutomation.Visible = false;
            btn_cancelAutomation.Visible = true;
            btn_Capture_StartStop.Enabled = false;

            if (performReferenceDrive)
            {
                appendToAutomationLog("\nPerforming reference drive...");
                this.Refresh();
                if (!control.referenceDrive())
                {
                    appendToAutomationLog("failure!\n");
                    finalizeAutomation(control, tracker);
                    return;
                }
                appendToAutomationLog("done\n");
            }

            int taskCtr = 1;
            foreach (st_Automation_Task task in tasks)
            {
                appendToAutomationLog("\n" + DateTime.Now.ToString() + ": Starting Task " + + taskCtr++ + " of " + tasks.Count + ".\n");
                printDebugMessage("Starting Task " + +taskCtr++ + " of " + tasks.Count);

                //Config File
                if (task.configFile.Length > 1)
                {
                    appendToAutomationLog("Load config file: " + task.configFile + "...");
                    if (!loadConfigurationFile(task.configFile, sender, e))
                    {
                        appendToAutomationLog("failed! Not in a valid format!\n");
                        finalizeAutomation(control, tracker);
                        return;
                    }
                    appendToAutomationLog("done\n");
                    appendToAutomationLog("Writing config to Board...");
                    //check ping first
                    if (!com.openCOM(SettingsCollector.DSP_Com))
                    {
                        appendToAutomationLog("failed! Can't open " + SettingsCollector.DSP_Com + "!\n");
                        finalizeAutomation(control, tracker);
                        return;
                    }
                    if (!com.pingBoard())
                    {
                        appendToAutomationLog("failed! Can't ping board.\n");
                        finalizeAutomation(control, tracker);
                        com.closeCOM();
                        return;
                    }
                    com.closeCOM();

                    btn_Sys_initSystem_Click(sender, e);
                    appendToAutomationLog("done\n");
                }
                this.Refresh();

                UInt32 startPos = task.startDistance;
                UInt32 actPos = startPos;
                UInt32 endPos = task.stopDistance;
                UInt32 steps = task.steps;
                UInt32 multiple_steps = task.multidataSteps;
                UInt32 nextMultiple = actPos;

                while (actPos <= endPos)
                {
                    appendToAutomationLog("\nGoto Position " + actPos.ToString() + "...");
                    printDebugMessage("Goto Position: " + actPos.ToString());
                    this.Refresh();
                    if (!gotoPosition(track, control, tracker, actPos))
                    {
                        appendToAutomationLog("Timeout\n");
                        finalizeAutomation(control, tracker);
                        return;
                    }
                    appendToAutomationLog("done\n");
                    printDebugMessage("Position: " + actPos.ToString() + " reached");
                    this.Refresh();
                    System.Threading.Thread.Sleep(BGW_Task.waitingTimeAfterDrive); //wait to prevent wall from swinging

                    if (preCaptures > 0)
                    {
                        appendToAutomationLog("\nGetting PreCaptures (" + preCaptures + ")...");
                        printDebugMessage("Getting PreCaptures: " + preCaptures.ToString());
                        this.Refresh();
                        BGW_Task.description = TASKDesc.PreCapture;
                        BGW_Task.captureCtr = 0;
                        BGW_Task.savePictures = captureRAWAndAndFFTDataTopngToolStripMenuItem.Checked;
                        BGW_Task.amount = preCaptures;
                        bgw.RunWorkerAsync();

                        while (bgw.IsBusy) //wait till job is done
                        {
                            this.Refresh();
                            Application.DoEvents();
                            System.Threading.Thread.Sleep(100);
                        }

                        if (automationCancelationPending)
                        {
                            appendToAutomationLog("failed!\nAutomation stopped!\n");
                            finalizeAutomation(control, tracker);
                            return;
                        }

                        appendToAutomationLog("done\n");

                        preCaptures = 0;
                    }

                    UInt32 cycles;
                    if (nextMultiple <= actPos)
                    {//multiple capture
                        appendToAutomationLog("Start multiple capture (" + task.multidataAmount.ToString() + " captures)!\n");
                        printDebugMessage("Start multiple capture (" + task.multidataAmount.ToString() + " captures)");
                        cycles = task.multidataAmount;
                        nextMultiple += multiple_steps;
                    }
                    else
                    {//single capture
                        appendToAutomationLog("Start single capture!\n");
                        printDebugMessage("Start single capture!");
                        cycles = 1;
                    }

                    this.Refresh();
                    double distance = tracker.getPosition();
                    printDebugMessage("Evaluated distance from tracker: " + distance.ToString());

                    if (!Directory.Exists(task.outputFolder))
                    {
                        Directory.CreateDirectory(task.outputFolder);
                    }
                    string folder = task.outputFolder + "\\" + actPos.ToString().PadLeft(5, '0');
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }

                    if (bgw.IsBusy != true)
                    {
                        BGW_Task.folder = folder;
                        BGW_Task.description = TASKDesc.Automation;
                        BGW_Task.measuredDistance = distance;
                        BGW_Task.captureCtr = 0;
                        BGW_Task.savePictures = captureRAWAndAndFFTDataTopngToolStripMenuItem.Checked;
                        BGW_Task.amount = cycles;
                        bgw.RunWorkerAsync();
                        btn_Capture_StartStop.Enabled = false;
                    }
                    else
                    {
                        appendToAutomationLog("Background worker still active!\n");
                        finalizeAutomation(control, tracker);
                        return;
                    }

                    actPos += steps;

                    while (bgw.IsBusy) //wait till job is done
                    {
                        this.Refresh();
                        Application.DoEvents();
                        System.Threading.Thread.Sleep(100);
                    }

                    if (automationCancelationPending)
                    {
                        appendToAutomationLog("Automation stopped before finishing all tasks!\n");
                        finalizeAutomation(control, tracker);
                        return;
                    }
                }
            }

            appendToAutomationLog("\nClosing " + positionTrackerCOM + ".\n");
            tracker.closeCOM();
            appendToAutomationLog("Closing " + positionControlCOM + ".\n");
            control.closeCOM();

            appendToAutomationLog("Automation finished: " + DateTime.Now.ToString());
            printDebugMessage("Automation finished");

            finalizeAutomation();
        }
    }
}
