using System;
using System.Windows.Forms;
using EH.DSP;
using System.Windows.Forms.DataVisualization.Charting;
using EH.RadarControl;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Globalization;
using System.Diagnostics;
using Radar_Config_and_Measurement_Tool.Properties;

namespace Radar_Config_and_Measurement_Tool
{
    public partial class Main : Form
    {
        #region Local Variables
        private const double c0 = 299701743;//<- c air    299792458;
        private DSP_Comm com;

        enum TASKDesc { InfiniteCapture_noCSV, Capture2CSV_Amount, Capture2CSV_ExternalStop, PreCapture, Automation };

        struct st_Automation_Task
        {
            public string outputFolder;
            public string configFile;
            public UInt32 startDistance;
            public UInt32 stopDistance;
            public UInt32 steps;
            public UInt32 multidataSteps;
            public UInt32 multidataAmount;
        }

        struct st_BGW_Task
        {
            public TASKDesc description;
            public UInt32 captureCtr;
            public UInt32 amount;

            //CSV+Automation
            public string folder;

            //Automation
            public double measuredDistance;

            //FormClosing pending!
            public bool closing_pending;

            //Should data be saved to pictures?
            public bool savePictures;

            public Int32 waitingTimeAfterDrive;
        }
        private st_BGW_Task BGW_Task;

        #endregion

        #region Local functions
        private bool gotoPosition(SettingsCollector.Track track, PositionControl control, PositionTracker tracker, UInt32 position)
        {
            if (!control.setPosition(position))
            {
                return false;
            }

            switch (track)
            {
                case SettingsCollector.Track.MillimeterTrack:
                    bool finished = false;
                    double actPosD = position;

                    int tmr = 0;
                    do
                    {
                        tmr++;
                        double pos = tracker.getPosition();
                        if (pos > actPosD - 5 && pos < actPosD + 5)
                        {
                            finished = true;
                        }
                        System.Threading.Thread.Sleep(100);
                    }
                    while (!finished & tmr < 30000); //5 min timeout
                    //System.Threading.Thread.Sleep(1000);

                    if (tmr >= 600)
                        return false;

                    break;

                case SettingsCollector.Track.FEW_Lab:
                    int tout = 600; //wait upto 1 minute
                    int ctr = 2; //retry twice
                    do
                    {
                        tout--;
                        if (tout == 0)
                        {
                            if (ctr > 0)
                            {
                                control.setPosition(position);
                                ctr--;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        System.Threading.Thread.Sleep(100);
                    } while (((Lab_PositionControl)control).getPosition() != position); //wait till position is reached
                    
                    break;

                case SettingsCollector.Track.Uni_Ulm:
                    int tout_uni = 600; //wait upto 1 minute
                    int ctr_uni = 2; //retry twice

                    //+- 2 because of rounding errors
                    UInt32 actPos = 0;
                    UInt32 minPos = 0;
                    if(position > 2)
                        minPos = position-2;
                    UInt32 maxPos = position + 2;

                    do
                    {
                        tout_uni--;
                        if (tout_uni == 0)
                        {
                            if (ctr_uni > 0)
                            {
                                control.setPosition(position);
                                ctr_uni--;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        System.Threading.Thread.Sleep(100);
                        actPos = ((UniUlm_PositionControl)control).getPosition();
                    } while (actPos < minPos || actPos > maxPos); //wait till position is reached
                    break;

                case SettingsCollector.Track.SimulatorTrack:
                    break;

                default:
                    return false;
            }
            return true;
        }
        #endregion

        #region Start-Close Programm
        public Main()
        {
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            Console.WriteLine("Initialization is started");
            InitializeComponent();
            Console.WriteLine("Initialization was successful");

            Settings setting = new Settings();
            num_TrackControl_Lab_MaxDist.Value = setting.iselMaxDist;
            num_TrackControl_Lab_Steps.Value = setting.iselStepsize;

            SettingsCollector.init(setting);

            cB_cap2CSV.Checked = setting.cap2CSV_Enabled;
            num_Cap2CSV_amount.Value = setting.cap2CSV_amount;
            num_Cap2CSV_seconds.Value = setting.cap2CSV_time;
            switch (setting.cap2CSV_Selection)
            {
                case 0:
                    rB_CaptureCSV_manual.Checked = true;
                    break;
                case 1:
                    rB_CaptureCSV_time.Checked = true;
                    break;
                default:
                    rB_CaptureCSV_amount.Checked = true;
                    break;
            }
            captureRAWAndAndFFTDataTopngToolStripMenuItem.Checked = setting.capturePNG;
            cB_cap2CSV_CheckedChanged(this, null);
            num_Capture_CycleDelay.Value = setting.captureCycleDelay;
            BGW_Task.waitingTimeAfterDrive = setting.AutomationWaitingTime;
            BGW_Task.closing_pending = false;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                Version v = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
                this.Text = "Radar Config and Measurement Tool V" + v.Major + "." + v.Minor + "." + v.Build;
            }
            else
            {
                Version v = Assembly.GetExecutingAssembly().GetName().Version;
                this.Text = "Radar Config and Measurement Tool Portable (V" + v.Major + "." + v.Minor + "." + v.Build + ")";
            }
            
            com = new DSP_Comm();

            //Initial setup -> setup for actual device will be overwritten later!
            updateTabs(true); //show all tabs
            //shiftRRC01 settings
            gB_RRC01_Settings.Location = gB_HMC_obtainedValues.Location;
            cB_AD5930_tINT_Multiplier.SelectedIndex = 0;
            cB_AD5930_TBURST_Multiplier.SelectedIndex = 0;
            cB_Sys_PLL_Prescale.SelectedIndex = 0;
            cB_Sys_window.SelectedIndex = 1;
            cB_HMC_ModType.SelectedIndex = 1;
            cB_HMC_SDMode.SelectedIndex = 5;
            cB_HMC_DSM_source.SelectedIndex = 0;
            cB_HMC_LockDetect.SelectedIndex = 5;
            cB_HMC_muxout.SelectedIndex = 0;
            cB_BGT_AnMUX.SelectedIndex = 0;
            cB_BGT_LNAgain.SelectedIndex = 0;
            cB_BGT_TX_Atten.SelectedIndex = 0;
            num_Sys_Window_PolOrder_ValueChanged(sender, e);
            AD5930_ValueChanged(sender, e);
            ADF4108_ValueChanged(sender, e);
            HMC_ValueChanged(sender, e);

            //disable advanced Settings
            enableAdvancedSettingsToolStripMenuItem.Checked = true; //because it will change after click handler!
            enableAdvancedSettingsToolStripMenuItem_Click(sender, e);

            //load last setup
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "lastConfig.rcfg"))
                loadConfigurationFile(AppDomain.CurrentDomain.BaseDirectory + "lastConfig.rcfg", sender, e, true);
            
            updateSysInfoBox(sender, e);
            updateTabSelection(sender, e);
            updateWindowParameter(sender, e);
            clearCharts();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveConfigurationFile(AppDomain.CurrentDomain.BaseDirectory + "lastConfig.rcfg", sender, e, true);
            
            if (bgw.IsBusy)
            {
                bgw.CancelAsync();
                this.Enabled = false;
                e.Cancel = true;
                BGW_Task.closing_pending = true;
                return;
            }
            com.closeCOM();

            Settings setting = new Settings();
            setting.iselMaxDist = num_TrackControl_Lab_MaxDist.Value;
            setting.iselStepsize = num_TrackControl_Lab_Steps.Value;
            setting.SelectedTrack = (uint)SettingsCollector.TrackSelection;
            setting.DSPCom = SettingsCollector.DSP_Com;
            setting.cap2CSV_Enabled = cB_cap2CSV.Checked;
            setting.cap2CSV_amount = num_Cap2CSV_amount.Value;
            setting.cap2CSV_time = num_Cap2CSV_seconds.Value;
            if (rB_CaptureCSV_manual.Checked)
                setting.cap2CSV_Selection = 0;
            else if (rB_CaptureCSV_time.Checked)
                setting.cap2CSV_Selection = 1;
            else
                setting.cap2CSV_Selection = 2;

            setting.captureCycleDelay = num_Capture_CycleDelay.Value;
            setting.capturePNG = captureRAWAndAndFFTDataTopngToolStripMenuItem.Checked;
            setting.AutomationWaitingTime = BGW_Task.waitingTimeAfterDrive;
            setting.Save();
        }
        #endregion

        #region TabControl
        private void updateTabSelection(object sender, EventArgs e)
        {
            //update tabs
            newBoardSelection(SettingsCollector.BoardSelection);
            newTrackSelection(SettingsCollector.TrackSelection);
        }

        private void updateTabs(bool showALL = false)
        {
            //save last Tab
            TabPage lastTab = tC_Main.SelectedTab;

            if (showALL)
            {
                tC_Main.TabPages.Clear();
                tC_Main.TabPages.Add(tP_System_Config);
                tC_Main.TabPages.Add(tP_HMC703_Config);
                tC_Main.TabPages.Add(tP_ADF4108_Config);
                tC_Main.TabPages.Add(tP_AD5930_Config);
                tC_Main.TabPages.Add(tP_BGT24_Config);
                gB_Track_FEW.Visible = true;
                gB_Track_Mill.Visible = true;
                gB_HMC_obtainedValues.Visible = true;
                gB_RRC01_Settings.Visible = true;
                gB_RRC160_Settings.Visible = true;
                panel_Sys_PLL_CTR.Visible = true;
                num_Sys_freqMult.Visible = true;
                num_Sys_bias1.Visible = true;
                lbl_Sys_bias1.Visible = true;
                tC_Main.TabPages.Add(tP_TrackControl);
                tC_Main.TabPages.Add(tP_SingleCapture);
                tC_Main.TabPages.Add(tP_Automation);
            }
            else
            {
                tC_Main.TabPages.Clear();
                tC_Main.TabPages.Add(tP_System_Config);
                tP_ADF4108_Config.Text = "ADF4108";
                lbl_Sys_FrequMult.Text = "Frequency multipl.:";
                switch (SettingsCollector.BoardSelection)
                {
                    case SettingsCollector.Board.FractionalN_PLL:
                        tC_Main.TabPages.Add(tP_HMC703_Config);
                        gB_HMC_obtainedValues.Visible = true;
                        gB_RRC01_Settings.Visible = false;
                        gB_RRC160_Settings.Visible = false;
                        lbl_Sys_SystemClock.Text = "Sys. Clock (PLL Xref):";
                        panel_Sys_PLL_CTR.Visible = false;
                        num_Sys_bias1.Visible = true;
                        lbl_Sys_bias1.Visible = true;
                        num_Sys_freqMult.Visible = true;
                        break;
                    case SettingsCollector.Board.RRC01:
                        tC_Main.TabPages.Add(tP_ADF4108_Config);
                        tC_Main.TabPages.Add(tP_AD5930_Config);
                        gB_HMC_obtainedValues.Visible = false;
                        gB_RRC01_Settings.Visible = true;
                        gB_RRC160_Settings.Visible = false;
                        lbl_Sys_SystemClock.Text = "Sys. Clock (DDS):";
                        panel_Sys_PLL_CTR.Visible = true;
                        num_Sys_bias1.Visible = true;
                        lbl_Sys_bias1.Visible = true;
                        num_Sys_freqMult.Visible = true;
                        break;
                    case SettingsCollector.Board.TankGauging:
                        tC_Main.TabPages.Add(tP_ADF4108_Config);
                        tC_Main.TabPages.Add(tP_AD5930_Config);
                        gB_HMC_obtainedValues.Visible = false;
                        gB_RRC01_Settings.Visible = false;
                        gB_RRC160_Settings.Visible = false;
                        lbl_Sys_SystemClock.Text = "Sys. Clock (DDS):";
                        panel_Sys_PLL_CTR.Visible = true;
                        num_Sys_bias1.Visible = true;
                        lbl_Sys_bias1.Visible = true;
                        num_Sys_freqMult.Visible = true;
                        break;
                    case SettingsCollector.Board.BGT24:
                        tC_Main.TabPages.Add(tP_ADF4108_Config);
                        tC_Main.TabPages.Add(tP_AD5930_Config);
                        tC_Main.TabPages.Add(tP_BGT24_Config);
                        gB_HMC_obtainedValues.Visible = false;
                        gB_RRC01_Settings.Visible = false;
                        gB_RRC160_Settings.Visible = false;
                        lbl_Sys_SystemClock.Text = "Sys. Clock (DDS):";
                        panel_Sys_PLL_CTR.Visible = true;
                        num_Sys_bias1.Visible = true;
                        lbl_Sys_bias1.Visible = true;
                        num_Sys_freqMult.Visible = true;
                        break;
                    case SettingsCollector.Board.RRC160_IntN:
                        tC_Main.TabPages.Add(tP_ADF4108_Config);
                        tC_Main.TabPages.Add(tP_AD5930_Config);
                        gB_HMC_obtainedValues.Visible = false;
                        gB_RRC01_Settings.Visible = false;
                        gB_RRC160_Settings.Visible = true;
                        lbl_Sys_SystemClock.Text = "Sys. Clock (DDS):";
                        panel_Sys_PLL_CTR.Visible = true;
                        num_Sys_bias1.Visible = false;
                        lbl_Sys_bias1.Visible = false;
                        num_Sys_freqMult.Visible = true;
                        break;
                    case SettingsCollector.Board.RRC160_FracN:
                        tC_Main.TabPages.Add(tP_HMC703_Config);
                        gB_HMC_obtainedValues.Visible = true;
                        gB_RRC01_Settings.Visible = false;
                        gB_RRC160_Settings.Visible = true;
                        lbl_Sys_SystemClock.Text = "Sys. Clock (PLL Xref):";
                        panel_Sys_PLL_CTR.Visible = false;
                        num_Sys_bias1.Visible = false;
                        lbl_Sys_bias1.Visible = false;
                        num_Sys_freqMult.Visible = true;
                        break;
                    case SettingsCollector.Board.FindMine:
                        tP_ADF4108_Config.Text = "ADF4113";
                        tC_Main.TabPages.Add(tP_ADF4108_Config);
                        tC_Main.TabPages.Add(tP_AD5930_Config);
                        gB_HMC_obtainedValues.Visible = false;
                        gB_RRC01_Settings.Visible = false;
                        gB_RRC160_Settings.Visible = false;
                        lbl_Sys_SystemClock.Text = "Sys. Clock (DDS):";
                        panel_Sys_PLL_CTR.Visible = true;
                        num_Sys_bias1.Visible = true;
                        lbl_Sys_bias1.Visible = true;
                        num_Sys_freqMult.Visible = false;
                        lbl_Sys_FrequMult.Text = "x2 path enabled:";
                        break;
                    default:
                        throw new NotImplementedException();
                }

                toolStripStatusLabel_Module.Text = SettingsCollector.BoardName();

                if (enableAdvancedSettingsToolStripMenuItem.Checked)
                {
                    tC_Main.TabPages.Add(tP_TrackControl);
                    switch (SettingsCollector.TrackSelection)
                    {
                        case SettingsCollector.Track.MillimeterTrack:
                            gB_Track_FEW.Visible = false;
                            gB_Track_Mill.Visible = true;
                            break;
                        case SettingsCollector.Track.FEW_Lab:
                            gB_Track_FEW.Visible = true;
                            gB_Track_FEW_MotorSettings.Visible = false;
                            gB_Track_FEW_Position.Visible = true;
                            gB_Track_Mill.Visible = false;
                            break;
                        case SettingsCollector.Track.Uni_Ulm:
                            gB_Track_FEW.Visible = true;
                            gB_Track_FEW_MotorSettings.Visible = true;
                            gB_Track_FEW_Position.Visible = false;
                            gB_Track_Mill.Visible = false;
                            break;
                        case SettingsCollector.Track.SimulatorTrack:
                            gB_Track_FEW.Visible = false;
                            gB_Track_Mill.Visible = false;
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }

                toolStripStatusLabel_Track.Text = SettingsCollector.TrackName();

                tC_Main.TabPages.Add(tP_SingleCapture);
                tC_Main.TabPages.Add(tP_Automation);

                updateSysInfoBox(this, null);
            }

            //restore Tab
            if (tC_Main.Contains(lastTab))
            {
                tC_Main.SelectedTab = lastTab;
            }
        }

        private void newBoardSelection(SettingsCollector.Board newBoardSelection)
        {
            tankGaugingToolStripMenuItem.Checked = false;
            bGT24MTR11ToolStripMenuItem.Checked = false;
            RRC01DEToolStripMenuItem.Checked = false;
            fractionalNPLLFToolStripMenuItem.Checked = false;
            rRC160DDSGToolStripMenuItem.Checked = false;
            rRC160FracNHToolStripMenuItem.Checked = false;
            findMineToolStripMenuItem.Checked = false;

            switch (newBoardSelection)
            {
                case SettingsCollector.Board.TankGauging:
                    tankGaugingToolStripMenuItem.Checked = true;
                    break;
                case SettingsCollector.Board.BGT24:
                    bGT24MTR11ToolStripMenuItem.Checked = true;
                    break;
                case SettingsCollector.Board.RRC01:
                    RRC01DEToolStripMenuItem.Checked = true;
                    break;
                case SettingsCollector.Board.FractionalN_PLL:
                    fractionalNPLLFToolStripMenuItem.Checked = true;
                    break;
                case SettingsCollector.Board.RRC160_IntN:
                    rRC160DDSGToolStripMenuItem.Checked = true;
                    break;
                case SettingsCollector.Board.RRC160_FracN:
                    rRC160FracNHToolStripMenuItem.Checked = true;
                    break;
                case SettingsCollector.Board.FindMine:
                    findMineToolStripMenuItem.Checked = true;
                    break;
                default:
                    throw new NotImplementedException();
            }

            SettingsCollector.BoardSelection = newBoardSelection;
            updateTabs();
        }

        private void RRC01DEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newBoardSelection(SettingsCollector.Board.RRC01);
            if (MessageBox.Show("Do you want to load the default configuration?", "Default Config", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                loadDefaultConfigurationToolStripMenuItem_Click(sender, e);
        }

        private void fractionalNPLLFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newBoardSelection(SettingsCollector.Board.FractionalN_PLL);
            if (MessageBox.Show("Do you want to load the default configuration?", "Default Config", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                loadDefaultConfigurationToolStripMenuItem_Click(sender, e);
        }

        private void tankGaugingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newBoardSelection(SettingsCollector.Board.TankGauging);
            if (MessageBox.Show("Do you want to load the default configuration?", "Default Config", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                loadDefaultConfigurationToolStripMenuItem_Click(sender, e);
        }

        private void bGT24MTR11ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newBoardSelection(SettingsCollector.Board.BGT24);
            if (MessageBox.Show("Do you want to load the default configuration?", "Default Config", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                loadDefaultConfigurationToolStripMenuItem_Click(sender, e);
        }

        private void rRC160DDSGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newBoardSelection(SettingsCollector.Board.RRC160_IntN);
            if (MessageBox.Show("Do you want to load the default configuration?", "Default Config", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                loadDefaultConfigurationToolStripMenuItem_Click(sender, e);
        }

        private void rRC160FracNHToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newBoardSelection(SettingsCollector.Board.RRC160_FracN);
            if (MessageBox.Show("Do you want to load the default configuration?", "Default Config", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                loadDefaultConfigurationToolStripMenuItem_Click(sender, e);
        }

        private void findMineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newBoardSelection(SettingsCollector.Board.FindMine);
            if (MessageBox.Show("Do you want to load the default configuration?", "Default Config", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                loadDefaultConfigurationToolStripMenuItem_Click(sender, e);
        }

        private void newTrackSelection(SettingsCollector.Track newTrackSelection)
        {
            millimeterTrackSchlagetermattToolStripMenuItem.Checked = false;
            fEWLabToolStripMenuItem.Checked = false;
            uniUlmToolStripMenuItem.Checked = false;

            switch (newTrackSelection)
            {
                case SettingsCollector.Track.MillimeterTrack:
                    millimeterTrackSchlagetermattToolStripMenuItem.Checked = true;
                    break;
                case SettingsCollector.Track.FEW_Lab:
                    fEWLabToolStripMenuItem.Checked = true;
                    num_FEW_Motor_Pos.Maximum = 2688;
                    lbl_FEW_Pos.Text = "Position: 0 ... 2688";
                    break;
                case SettingsCollector.Track.Uni_Ulm:
                    uniUlmToolStripMenuItem.Checked = true;
                    num_TrackControl_Lab_MaxDist_ValueChanged(this, null);
                    break;
                case SettingsCollector.Track.SimulatorTrack:
                    //Simulation Automation active
                    break;
                default:
                    throw new NotImplementedException();
            }

            SettingsCollector.TrackSelection = newTrackSelection;
            updateTabs();
        }

        private void millimeterTrackSchlagetermattToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newTrackSelection(SettingsCollector.Track.MillimeterTrack);
        }

        private void fEWLabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newTrackSelection(SettingsCollector.Track.FEW_Lab);
        }

        private void uniUlmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newTrackSelection(SettingsCollector.Track.Uni_Ulm);
        }

        private void simulationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newTrackSelection(SettingsCollector.Track.SimulatorTrack);
        }
        #endregion

        #region ToolStripMenu
        private void captureRAWAndAndFFTDataTopngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            captureRAWAndAndFFTDataTopngToolStripMenuItem.Checked = !captureRAWAndAndFFTDataTopngToolStripMenuItem.Checked;
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().Show();
        }

        private void createAutomationScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AutomationScriptCreator(num_TrackControl_Lab_Steps.Value,num_TrackControl_Lab_MaxDist.Value).Show();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void selectDSPComPortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new DSP_ComSelect().Show();
        }

        private void settingsToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            selectDSPComPortToolStripMenuItem.Text = "Change DSP Com (" + SettingsCollector.DSP_Com + ")";
        }

        private void enableAdvancedSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateTabs(true); //show all tabs

            if (!enableAdvancedSettingsToolStripMenuItem.Checked)
            {
                enableAdvancedSettingsToolStripMenuItem.Checked = true;

                //System Config
                num_Sys_freqMult.Enabled = true;
                num_Sys_freqMult.BackColor = Color.PeachPuff;
                num_Sys_SysClock.Enabled = true;
                num_Sys_SysClock.BackColor = Color.PeachPuff;
                num_Sys_SampleClock.Enabled = true;
                num_Sys_SampleClock.BackColor = Color.PeachPuff;
                num_Sys_extFreqInc.Enabled = true;
                num_Sys_extFreqInc.BackColor = Color.PeachPuff;
                num_Sys_VCOPrescaler.Enabled = true;
                num_Sys_VCOPrescaler.BackColor = Color.PeachPuff;
                num_Sys_RRC01_Vref_DAC.Enabled = true;
                num_Sys_RRC01_Vref_DAC.BackColor = Color.PeachPuff;
                num_Sys_RRC160_OffsetFrequency.Enabled = true;
                num_Sys_RRC160_OffsetFrequency.BackColor = Color.PeachPuff;

                //AD5930
                cB_AD5930_DeltaF_negative.Enabled = true;
                cB_AD5930_DeltaF_negative.BackColor = Color.PeachPuff;

                //HMC
                for (int i = 0; i <= 10; i++)
                {
                    string objectName = "cB_HMC_RST_" + i.ToString("00");
                    CheckBox cB = this.Controls.Find(objectName, true).FirstOrDefault() as CheckBox;
                    cB.Enabled = true;
                    cB.BackColor = Color.PeachPuff;
                }
                num_HMC_RefDiv.Enabled = true;
                num_HMC_RefDiv.BackColor = Color.PeachPuff;
                num_HMC_Seed.Enabled = true;
                num_HMC_Seed.BackColor = Color.PeachPuff;
                cB_HMC_autoseed.Enabled = true;
                cB_HMC_autoseed.BackColor = Color.PeachPuff;
                cB_HMC_extTrig.Enabled = true;
                cB_HMC_extTrig.BackColor = Color.PeachPuff;
                cB_HMC_forceDSM.Enabled = true;
                cB_HMC_forceDSM.BackColor = Color.PeachPuff;
                cB_HMC_forceRDIV.Enabled = true;
                cB_HMC_forceRDIV.BackColor = Color.PeachPuff;
                cB_HMC_disExtACCReset.Enabled = true;
                cB_HMC_disExtACCReset.BackColor = Color.PeachPuff;
                cB_HMC_singleStepRamp.Enabled = true;
                cB_HMC_singleStepRamp.BackColor = Color.PeachPuff;
                cB_HMC_SDMode.Enabled = true;
                lbl_HMC_SDMode.BackColor = Color.PeachPuff;
                cB_HMC_DSM_source.Enabled = true;
                lbl_HMC_DSMClock.BackColor = Color.PeachPuff;
                cB_HMC_trainLD.Enabled = true;
                cB_HMC_trainLD.BackColor = Color.PeachPuff;
                cB_HMC_LD_Counters.Enabled = true;
                cB_HMC_LD_Counters.BackColor = Color.PeachPuff;
                cB_HMC_LD_Timer.Enabled = true;
                cB_HMC_LD_Timer.BackColor = Color.PeachPuff;
                cB_HMC_cycleslipPrevention.Enabled = true;
                cB_HMC_cycleslipPrevention.BackColor = Color.PeachPuff;
                cB_HMC_LockDetect.Enabled = true;
                lbl_HMC_LockDetect.BackColor = Color.PeachPuff;
                int[] tmpAnEn = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 16, 17 };
                foreach (int i in tmpAnEn)
                {
                    string objectName = "cB_HMC_AnEn_" + i.ToString("00");
                    CheckBox cB = this.Controls.Find(objectName, true).FirstOrDefault() as CheckBox;
                    cB.Enabled = true;
                    cB.BackColor = Color.PeachPuff;
                }
                num_HMC_VCOOutBiasA.Enabled = true;
                num_HMC_VCOOutBiasA.BackColor = Color.PeachPuff;
                num_HMC_VCOOutBiasB.Enabled = true;
                num_HMC_VCOOutBiasB.BackColor = Color.PeachPuff;
                int[] tmpPD = { 3, 4, 5, 6, 7, 8, 9, 17, 18 };
                foreach (int i in tmpPD)
                {
                    string objectName = "cB_HMC_PD_" + i.ToString("00");
                    CheckBox cB = this.Controls.Find(objectName, true).FirstOrDefault() as CheckBox;
                    cB.Enabled = true;
                    cB.BackColor = Color.PeachPuff;
                }
                num_HMC_PD_Mcnt.Enabled = true;
                num_HMC_PD_Mcnt.BackColor = Color.PeachPuff;
                for (int i = 5; i <= 9; i++)
                {
                    string objectName = "cB_HMC_GPO_" + i.ToString("00");
                    CheckBox cB = this.Controls.Find(objectName, true).FirstOrDefault() as CheckBox;
                    cB.Enabled = true;
                    cB.BackColor = Color.PeachPuff;
                }

                //BGT24
                cB_BGT_disable16Div.Enabled = true;
                cB_BGT_disable16Div.BackColor = Color.PeachPuff;
                cB_BGT_disable64kDiv.Enabled = true;
                cB_BGT_disable64kDiv.BackColor = Color.PeachPuff;
            }
            else
            {
                //disable all advanced settings
                enableAdvancedSettingsToolStripMenuItem.Checked = false;

                //System Config
                num_Sys_freqMult.Enabled = false;
                num_Sys_freqMult.BackColor = SystemColors.Control;
                num_Sys_SysClock.Enabled = false;
                num_Sys_SysClock.BackColor = SystemColors.Control;
                num_Sys_SampleClock.Enabled = false;
                num_Sys_SampleClock.BackColor = SystemColors.Control;
                num_Sys_extFreqInc.Enabled = false;
                num_Sys_extFreqInc.BackColor = SystemColors.Control;
                num_Sys_VCOPrescaler.Enabled = false;
                num_Sys_VCOPrescaler.BackColor = SystemColors.Control;
                num_Sys_RRC01_Vref_DAC.Enabled = false;
                num_Sys_RRC01_Vref_DAC.BackColor = SystemColors.Control;
                num_Sys_RRC160_OffsetFrequency.Enabled = false;
                num_Sys_RRC160_OffsetFrequency.BackColor = SystemColors.Control;

                //AD5930
                cB_AD5930_DeltaF_negative.Enabled = false;
                cB_AD5930_DeltaF_negative.BackColor = Color.Transparent;

                //HMC
                for (int i = 0; i <= 10; i++)
                {
                    string objectName = "cB_HMC_RST_" + i.ToString("00");
                    CheckBox cB = this.Controls.Find(objectName, true).FirstOrDefault() as CheckBox;
                    cB.Enabled = false;
                    cB.BackColor = Color.Transparent;
                }
                num_HMC_RefDiv.Enabled = false;
                num_HMC_RefDiv.BackColor = SystemColors.Control;
                num_HMC_Seed.Enabled = false;
                num_HMC_Seed.BackColor = SystemColors.Control;
                cB_HMC_autoseed.Enabled = false;
                cB_HMC_autoseed.BackColor = Color.Transparent;
                cB_HMC_extTrig.Enabled = false;
                cB_HMC_extTrig.BackColor = Color.Transparent;
                cB_HMC_forceDSM.Enabled = false;
                cB_HMC_forceDSM.BackColor = Color.Transparent;
                cB_HMC_forceRDIV.Enabled = false;
                cB_HMC_forceRDIV.BackColor = Color.Transparent;
                cB_HMC_disExtACCReset.Enabled = false;
                cB_HMC_disExtACCReset.BackColor = Color.Transparent;
                cB_HMC_singleStepRamp.Enabled = false;
                cB_HMC_singleStepRamp.BackColor = Color.Transparent;
                cB_HMC_SDMode.Enabled = false;
                lbl_HMC_SDMode.BackColor = Color.Transparent;
                cB_HMC_DSM_source.Enabled = false;
                lbl_HMC_DSMClock.BackColor = Color.Transparent;
                cB_HMC_trainLD.Enabled = false;
                cB_HMC_trainLD.BackColor = Color.Transparent;
                cB_HMC_LD_Counters.Enabled = false;
                cB_HMC_LD_Counters.BackColor = Color.Transparent;
                cB_HMC_LD_Timer.Enabled = false;
                cB_HMC_LD_Timer.BackColor = Color.Transparent;
                cB_HMC_cycleslipPrevention.Enabled = false;
                cB_HMC_cycleslipPrevention.BackColor = Color.Transparent;
                cB_HMC_LockDetect.Enabled = false;
                lbl_HMC_LockDetect.BackColor = Color.Transparent;
                int[] tmpAnEn = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 16, 17 };
                foreach (int i in tmpAnEn)
                {
                    string objectName = "cB_HMC_AnEn_" + i.ToString("00");
                    CheckBox cB = this.Controls.Find(objectName, true).FirstOrDefault() as CheckBox;
                    cB.Enabled = false;
                    cB.BackColor = Color.Transparent;
                }
                num_HMC_VCOOutBiasA.Enabled = false;
                num_HMC_VCOOutBiasA.BackColor = SystemColors.Control;
                num_HMC_VCOOutBiasB.Enabled = false;
                num_HMC_VCOOutBiasB.BackColor = SystemColors.Control;
                int[] tmpPD = { 3, 4, 5, 6, 7, 8, 9, 17, 18 };
                foreach (int i in tmpPD)
                {
                    string objectName = "cB_HMC_PD_" + i.ToString("00");
                    CheckBox cB = this.Controls.Find(objectName, true).FirstOrDefault() as CheckBox;
                    cB.Enabled = false;
                    cB.BackColor = Color.Transparent;
                }
                num_HMC_PD_Mcnt.Enabled = false;
                num_HMC_PD_Mcnt.BackColor = SystemColors.Control;
                for (int i = 5; i <= 9; i++)
                {
                    string objectName = "cB_HMC_GPO_" + i.ToString("00");
                    CheckBox cB = this.Controls.Find(objectName, true).FirstOrDefault() as CheckBox;
                    cB.Enabled = false;
                    cB.BackColor = Color.Transparent;
                }

                //BGT24
                cB_BGT_disable16Div.Enabled = false;
                cB_BGT_disable16Div.BackColor = Color.Transparent;
                cB_BGT_disable64kDiv.Enabled = false;
                cB_BGT_disable64kDiv.BackColor = Color.Transparent;
            }
            updateTabs();
        }

        private void saveConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialogConfig.ShowDialog() == DialogResult.OK)
            {
                if(!saveConfigurationFile(saveFileDialogConfig.FileName, sender, e))
                {
                    MessageBox.Show("Saving configuration unsuccesfull!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void loadConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialogConfig.ShowDialog() == DialogResult.OK)
            {
                if (!loadConfigurationFile(openFileDialogConfig.FileName, sender, e))
                {
                    MessageBox.Show("Not a valid format.\nLoading canceled.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void loadDefaultConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte[] embeddedResource = null;

            switch (SettingsCollector.BoardSelection)
            {
                case SettingsCollector.Board.TankGauging:
                    embeddedResource = Properties.Resources.TG_DefaultConfig;
                    break;
                case SettingsCollector.Board.BGT24:
                    embeddedResource = Properties.Resources.BGT24_DefaultConfig;
                    break;
                case SettingsCollector.Board.RRC01:
                    embeddedResource = Properties.Resources.RRC01_DefaultConfig;
                    break;
                case SettingsCollector.Board.FractionalN_PLL:
                    embeddedResource = Properties.Resources.FracN_DefaultConfig;
                    break;
                case SettingsCollector.Board.RRC160_IntN:
                    embeddedResource = Properties.Resources.RRC160_DDS_DefaultConfig;
                    break;
                case SettingsCollector.Board.RRC160_FracN:
                    embeddedResource = Properties.Resources.RRC160_FracN_DefaultConfig;
                    break;
                case SettingsCollector.Board.FindMine:
                    embeddedResource = Properties.Resources.FindMine;
                    break;
                default:
                    throw new NotImplementedException();
            }

            String filePath = "";
            try
            {
                filePath = Path.GetTempPath() + "defaultSetup.rcfg";
                File.WriteAllBytes(filePath, embeddedResource);

            }
            catch (Exception)
            {
                MessageBox.Show("Can't open documentation.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            loadConfigurationFile(filePath, this, null);
        }

        private void documentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                String openPDFFile = Path.GetTempPath() + "RCM_Doc.pdf";
                File.WriteAllBytes(openPDFFile, Properties.Resources.DocumentationRCM);
                Process.Start(openPDFFile);
            }
            catch (Exception)
            {
                MessageBox.Show("Can't open documentation.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void evaluateWithMatlabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK)
            {
                string final_path = fbd.SelectedPath;
                if (!Directory.Exists(final_path))
                {
                    Directory.CreateDirectory(final_path);
                }
                try
                {
                    //Save to temporaray folder first
                    String tmp_path = Path.GetTempPath() + "RCM_MatFiles";
                    if (!Directory.Exists(tmp_path))
                    {
                        Directory.CreateDirectory(tmp_path);
                    }
                    File.WriteAllBytes(tmp_path + "\\c0.m", Properties.Resources.Matlab_c0);
                    File.WriteAllBytes(tmp_path + "\\dft_us.m", Properties.Resources.Matlab_dft_us);
                    File.WriteAllBytes(tmp_path + "\\dispstat.m", Properties.Resources.Matlab_dispstat);
                    File.WriteAllBytes(tmp_path + "\\fmcweval.m", Properties.Resources.Matlab_fmcweval);
                    File.WriteAllBytes(tmp_path + "\\fmcweval_ensemble.m", Properties.Resources.Matlab_fmcweval_ensemble);
                    File.WriteAllBytes(tmp_path + "\\fmcweval_maxima_detection.m", Properties.Resources.Matlab_fmcweval_maxima_detection);
                    File.WriteAllBytes(tmp_path + "\\fmcweval_range.m", Properties.Resources.Matlab_fmcweval_range);
                    File.WriteAllBytes(tmp_path + "\\fmcwevalini_default_RCM.m", Properties.Resources.Matlab_fmcwevalini_default_RCM);
                    File.WriteAllBytes(tmp_path + "\\fmcwrun_from_RCM_Tool.m", Properties.Resources.Matlab_fmcwrun_from_RCM_Tool);
                    File.WriteAllBytes(tmp_path + "\\fmcwrun_from_RCM_Tool_Kondensat.m", Properties.Resources.Matlab_fmcwrun_from_RCM_Tool_Kondensat);
                    File.WriteAllBytes(tmp_path + "\\fmcwrun_from_RCM_Tool_Linearity.m", Properties.Resources.Matlab_fmcwrun_from_RCM_Tool_Linearity);
                    File.WriteAllBytes(tmp_path + "\\fmcwrun_from_RCM_Tool_TimeBased.m", Properties.Resources.Matlab_fmcwrun_from_RCM_Tool_TimeBased);
                    File.WriteAllBytes(tmp_path + "\\huder.m", Properties.Resources.Matlab_huder);
                    File.WriteAllBytes(tmp_path + "\\suptitle.m", Properties.Resources.Matlab_suptitle);

                    //Use windows copy (so that overwite question will be shown)
                    //Directory.GetFiles(tmp_path).ToList().ForEach(f => File.Copy(f, final_path + "\\" + f));
                    Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(tmp_path, final_path, Microsoft.VisualBasic.FileIO.UIOption.AllDialogs);

                    //delete temporaray data
                    Directory.Delete(tmp_path, true);
                }
                catch (Exception)
                {
                    MessageBox.Show("Can't save Matlab evaluation files.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                result = MessageBox.Show("Files saved successfully.\nDo you want to start Matlab evalution?", "Start Matlab?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        Process.Start(final_path + "\\fmcwrun_from_RCM_Tool.m");
                        Process.Start(final_path + "\\fmcwrun_from_RCM_Tool_Linearity.m");
                        Process.Start(final_path + "\\fmcwrun_from_RCM_Tool_TimeBased.m");
                        Process.Start(final_path + "\\fmcwrun_from_RCM_Tool_Kondensat.m");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Can't open matlab file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            
        }

        private void getModuleInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            char module;
            UInt16 version;

            if (!com.openCOM(SettingsCollector.DSP_Com))
            {
                MessageBox.Show("Can't open " + SettingsCollector.DSP_Com + "!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!com.getBoardModuleAndVersion(out module, out version))
            {
                MessageBox.Show("Can't connect to board!\n Please check connection.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Connected DSP module:\n\nModule:\t\t" + module + "\nVersion:\t\t" + version, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            com.closeCOM();
        }

        private void enableAutomationDebugOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            enableAutomationDebugOutputToolStripMenuItem.Checked = !enableAutomationDebugOutputToolStripMenuItem.Checked;
        }

        private void setAutomationWaitingTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NumericInputDialog testDialog = new NumericInputDialog();

            testDialog.num_Value.Value = BGW_Task.waitingTimeAfterDrive;

            // Show testDialog as a modal dialog and determine if DialogResult = OK.
            if (testDialog.ShowDialog(this) == DialogResult.OK)
            {
                // Read the contents of testDialog's TextBox.
                BGW_Task.waitingTimeAfterDrive = (Int32)testDialog.num_Value.Value;
            }
            testDialog.Dispose();
        }

        private void advancedToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            setAutomationWaitingTimeToolStripMenuItem.Text = "Set Automation Waiting Time (" + BGW_Task.waitingTimeAfterDrive.ToString() + " ms)";
        }
        
        #endregion 

    }  
}
