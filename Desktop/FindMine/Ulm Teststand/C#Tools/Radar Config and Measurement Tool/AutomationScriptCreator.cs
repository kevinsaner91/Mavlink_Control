using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using Radar_Config_and_Measurement_Tool.Properties;

namespace Radar_Config_and_Measurement_Tool
{
    public partial class AutomationScriptCreator : Form
    {
        private struct st_Task
        {
            public string outputFolder;
            public string configFile;
            public UInt32 startDistance;
            public UInt32 stopDistance;
            public UInt32 steps;
            public UInt32 multidataSteps;
            public UInt32 multidataAmount;

            public st_Task(string outputFolder, string configFile, UInt32 startDistance, UInt32 stopDistance, UInt32 steps, UInt32 multidataSteps, UInt32 multidataAmount)
            {
                this.outputFolder = outputFolder;
                this.configFile = configFile;
                this.startDistance = startDistance;
                this.stopDistance = stopDistance;
                this.steps = steps;
                this.multidataSteps = multidataSteps;
                this.multidataAmount = multidataAmount;
            }
        }

        private struct st_RadarAutomation
        {
            public Int32 trackID;
            public Int32 moduleID;
            public UInt32 posCOM;
            public UInt32 trackCOM;
            public bool refDrive;
            public UInt32 preCaptures;
            public List<st_Task> tasks;

            public double isel_stepsize;
            public UInt32 isel_end_distance;
        }

        private st_RadarAutomation script;
        
        public AutomationScriptCreator(decimal iselStepsize, decimal iselMaxDist)
        {
            InitializeComponent();

            num_isel_stepsize.Value = iselStepsize;
            num_isel_maxDist.Value = iselMaxDist;

        }

        private void initTasksTab()
        {
            cB_LoadConfig.Checked = false;
            tB_ConfigFile.Text = "";
            tB_OutFolder.Text = "";

            /*
             * Track ID:
             *   0 = Millimeter Track
             *   1 = FEW Lab
             *   2 = Uni Ulm
             *   3 = Simulation
             */

            switch (script.trackID)
            {
                case 0:
                    num_Start.Maximum = num_Stop.Maximum = num_Steps.Maximum = num_MultiSteps.Maximum = 30500;
                    break;
                case 1:
                    num_Start.Maximum = num_Stop.Maximum = num_Steps.Maximum = num_MultiSteps.Maximum = 2688;
                    break;
                case 2:
                    num_Start.Maximum = num_Stop.Maximum = num_Steps.Maximum = num_MultiSteps.Maximum = script.isel_end_distance;
                    break;
                case 3:
                default:
                    num_Start.Maximum = num_Stop.Maximum = num_Steps.Maximum = num_MultiSteps.Maximum = 999999;
                    break;
            }
        }

        private void AutomationScriptCreator_Load(object sender, EventArgs e)
        {
            tabControl1.TabPages.Clear();
            tabControl1.TabPages.Add(tabPage1);

            cB_Module.SelectedIndex = 0;
            cB_Track.SelectedIndex = 0;

            btn_nextCreate.Text = "next";

            cB_LoadConfig.Checked = true;
            cB_LoadConfig.Checked = false;
        }

        private void btn_nextCreate_Click(object sender, EventArgs e)
        {
            if (btn_nextCreate.Text.Contains("next"))
            {
                if (num_MotorCOM.Value == num_TrackerCOM.Value)
                {
                    MessageBox.Show("COM-Port of Motor Control and Tracker have to be different", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                script.moduleID = cB_Module.SelectedIndex;
                script.refDrive = cB_refDrive.Checked;
                script.preCaptures = (UInt32)num_preCaptures.Value;

                script.tasks = new List<st_Task>();
                script.tasks.Clear();

                if (cB_Track.Text.Equals("Uni Ulm"))
                {
                    script.trackID = cB_Track.SelectedIndex;
                    script.posCOM = (UInt32)num_MotorCOM.Value;
                    script.trackCOM = 0; // no tracker

                    //Ask for additional data
                    btn_nextCreate.Text = "ok";
                    tabControl1.TabPages.Clear();
                    tabControl1.TabPages.Add(tabPage3);
                }
                else
                {
                    if(cB_Track.Text.Equals("Simulation"))
                    {
                        script.trackID = 99;
                        script.posCOM = 1;
                        script.trackCOM = 2;
                    }
                    else 
                    {
                        script.trackID = cB_Track.SelectedIndex;
                        script.posCOM = (UInt32)num_MotorCOM.Value;
                        script.trackCOM = (UInt32)num_TrackerCOM.Value;
                    }

                    btn_nextCreate.Text = "create";
                    tabControl1.TabPages.Clear();
                    tabControl1.TabPages.Add(tabPage2);

                    lbl_TaskCount.Text = "0";
                    initTasksTab();
                }
            }
            else if (btn_nextCreate.Text.Contains("ok"))
            {
                script.isel_stepsize = (double)num_isel_stepsize.Value;
                script.isel_end_distance = (UInt32)num_isel_maxDist.Value;

                btn_nextCreate.Text = "create";
                tabControl1.TabPages.Clear();
                tabControl1.TabPages.Add(tabPage2);

                lbl_TaskCount.Text = "0";
                initTasksTab();
            }
            else
            {
                if (script.tasks.Count == 0)
                {
                    MessageBox.Show("No tasks created yet!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                //write XML
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;

                XmlWriter writer = XmlWriter.Create(saveFileDialog1.FileName, settings);
                writer.WriteStartDocument();
                writer.WriteComment("This file is an automation script for Radar Config and Measurement tool by M. Sautermeister/FET");
                writer.WriteComment("");
                writer.WriteComment("The Element RadarAutomation has got two Elements: Basic Setup and Tasks. Within the Element Tasks there one or more Task Elements that describe the different Automation steps.");
                writer.WriteStartElement("RadarAutomation");
                writer.WriteAttributeString("ScriptVersion", "1");

                writer.WriteComment("TrackID: 			MillimeterTrack = 0, FEW_Lab = 1, UniUlm = 2, Simulation = 99");
                writer.WriteComment("ModuleID: 			Tank Gauging = 0, RRC01 = 1, Fractional-N PLL = 2, BGT24 = 3");
                writer.WriteComment("PositionControlCOM:	COM-Port number of Position Motor Controller");
                writer.WriteComment("PositionTrackerCOM:	COM-Port number of Position Tracker/ Laser Tracker (0 if not used)");
                writer.WriteComment("PerformRefDrive:	\"True\" if reference drive should be perfomed before automation is started, else \"False\"");
                writer.WriteComment("PreCaptures:		The amount of not saved measurements before automation is started (to prevent false measurements on boot up process (gain adjustments))");
                writer.WriteComment("iselIMC4Steps:     Stepsize of isel-IMC4 Motor (Uni Ulm)");
                writer.WriteComment("iselIMC4MaxDist:   Maximum distance of isel-IMC4 Motor(Uni Ulm)");

                writer.WriteStartElement("BasicSetup");
                writer.WriteAttributeString("TrackID", script.trackID.ToString());
                writer.WriteAttributeString("ModuleID", script.moduleID.ToString());
                writer.WriteAttributeString("PositionControlCOM", script.posCOM.ToString());
                writer.WriteAttributeString("PositionTrackerCOM", script.trackCOM.ToString());
                writer.WriteAttributeString("PerformRefDrive", script.refDrive.ToString());
                writer.WriteAttributeString("PreCaptures", script.preCaptures.ToString());
                if (script.trackID == 2)
                {
                    writer.WriteAttributeString("iselIMC4Steps", script.isel_stepsize.ToString());
                    writer.WriteAttributeString("iselIMC4MaxDist", script.isel_end_distance.ToString());
                }
                writer.WriteEndElement();

                writer.WriteStartElement("Tasks");
                writer.WriteComment("OutputFolder: 		Folder where the captured data will be stored to");
                writer.WriteComment("ConfigFile: 		rcfg config file that will be used to configure the module before starting the automation task. Leave empty (\"\") if no new configuration should be loaded");
                writer.WriteComment("StartDistance: 		Start distance of the automation in mm");
                writer.WriteComment("StopDistance: 		Final distance of the automation in mm");
                writer.WriteComment("Steps: 				Stepssize in mm between two captures going from StartDistance to StopDistance");
                writer.WriteComment("MultidataSteps: 	The amount of captures that will be performed for distances that will be marked as multi-capture (by MultidataSteps)");

                foreach (st_Task task in script.tasks)
                {
                    writer.WriteStartElement("Task");
                    writer.WriteAttributeString("OutputFolder", task.outputFolder.ToString());
                    writer.WriteAttributeString("ConfigFile", task.configFile.ToString());
                    writer.WriteAttributeString("StartDistance", task.startDistance.ToString());
                    writer.WriteAttributeString("StopDistance", task.stopDistance.ToString());
                    writer.WriteAttributeString("Steps", task.steps.ToString());
                    writer.WriteAttributeString("MultidataSteps", task.multidataSteps.ToString());
                    writer.WriteAttributeString("MultidataAmount", task.multidataAmount.ToString());
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();

                writer.Flush();
                writer.Close();

                this.Close();
            }
        }

        private void cB_LoadConfig_CheckedChanged(object sender, EventArgs e)
        {
            tB_ConfigFile.Enabled = btn_SelConfigFile.Enabled = cB_LoadConfig.Checked;
        }

        private void btn_addTask_Click(object sender, EventArgs e)
        {
            UInt32 start = (UInt32)num_Start.Value;
            UInt32 stop = (UInt32)num_Stop.Value;
            UInt32 step = (UInt32)num_Steps.Value;
            UInt32 multiStep = (UInt32)num_MultiSteps.Value;
            UInt32 multiAmount = (UInt32)num_MultiAmount.Value;

            string folder = tB_OutFolder.Text;
            string config = "";
            bool enConfig = cB_LoadConfig.Checked;
            if (enConfig)
            {
                config = tB_ConfigFile.Text;
            }

            if(start > stop)
            {
                MessageBox.Show("Check following criteria:\nStart Distance > Stop Distance", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(folder.Length < 3) //at least drive letter has to be given
            {
                MessageBox.Show("No valid output folder is given!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (enConfig)
            {
                if (!File.Exists(config))
                {
                    DialogResult result = MessageBox.Show("Config not existing! Do you want to prceed anyway?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result == DialogResult.No)
                    {
                        return;
                    }
                }
            }

            st_Task task = new st_Task(folder, config, start, stop, step, multiStep, multiAmount);
            script.tasks.Add(task);

            lbl_TaskCount.Text = script.tasks.Count.ToString();
        }

        private void btn_selOutFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                tB_OutFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btn_SelConfigFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                tB_ConfigFile.Text = openFileDialog1.FileName;
            }
        }

        private void cB_Track_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cB_Track.Text.Equals("Uni Ulm"))
            {
                pnl_PosTracker.Visible = false;
            }
            else
            {
                pnl_PosTracker.Visible = true;
            }
        }
    }
}
