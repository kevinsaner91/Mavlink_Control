namespace Radar_Config_and_Measurement_Tool
{
    partial class AutomationScriptCreator
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutomationScriptCreator));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.pnl_PosTracker = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.num_TrackerCOM = new System.Windows.Forms.NumericUpDown();
            this.cB_refDrive = new System.Windows.Forms.CheckBox();
            this.num_preCaptures = new System.Windows.Forms.NumericUpDown();
            this.num_MotorCOM = new System.Windows.Forms.NumericUpDown();
            this.cB_Module = new System.Windows.Forms.ComboBox();
            this.cB_Track = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.num_Steps = new System.Windows.Forms.NumericUpDown();
            this.num_MultiAmount = new System.Windows.Forms.NumericUpDown();
            this.num_Stop = new System.Windows.Forms.NumericUpDown();
            this.num_MultiSteps = new System.Windows.Forms.NumericUpDown();
            this.num_Start = new System.Windows.Forms.NumericUpDown();
            this.cB_LoadConfig = new System.Windows.Forms.CheckBox();
            this.btn_SelConfigFile = new System.Windows.Forms.Button();
            this.btn_selOutFolder = new System.Windows.Forms.Button();
            this.tB_ConfigFile = new System.Windows.Forms.TextBox();
            this.tB_OutFolder = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btn_addTask = new System.Windows.Forms.Button();
            this.lbl_TaskCount = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.num_isel_maxDist = new System.Windows.Forms.NumericUpDown();
            this.num_isel_stepsize = new System.Windows.Forms.NumericUpDown();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.btn_nextCreate = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.pnl_PosTracker.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_TrackerCOM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_preCaptures)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_MotorCOM)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_Steps)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_MultiAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Stop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_MultiSteps)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Start)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_isel_maxDist)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_isel_stepsize)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(8, 6);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(545, 160);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.pnl_PosTracker);
            this.tabPage1.Controls.Add(this.cB_refDrive);
            this.tabPage1.Controls.Add(this.num_preCaptures);
            this.tabPage1.Controls.Add(this.num_MotorCOM);
            this.tabPage1.Controls.Add(this.cB_Module);
            this.tabPage1.Controls.Add(this.cB_Track);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(537, 134);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Basic Setup";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // pnl_PosTracker
            // 
            this.pnl_PosTracker.Controls.Add(this.label4);
            this.pnl_PosTracker.Controls.Add(this.num_TrackerCOM);
            this.pnl_PosTracker.Location = new System.Drawing.Point(14, 81);
            this.pnl_PosTracker.Name = "pnl_PosTracker";
            this.pnl_PosTracker.Size = new System.Drawing.Size(187, 41);
            this.pnl_PosTracker.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(114, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Position Tracker COM:";
            // 
            // num_TrackerCOM
            // 
            this.num_TrackerCOM.Location = new System.Drawing.Point(125, 11);
            this.num_TrackerCOM.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.num_TrackerCOM.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_TrackerCOM.Name = "num_TrackerCOM";
            this.num_TrackerCOM.Size = new System.Drawing.Size(50, 20);
            this.num_TrackerCOM.TabIndex = 8;
            this.num_TrackerCOM.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // cB_refDrive
            // 
            this.cB_refDrive.AutoSize = true;
            this.cB_refDrive.Location = new System.Drawing.Point(258, 93);
            this.cB_refDrive.Name = "cB_refDrive";
            this.cB_refDrive.Size = new System.Drawing.Size(135, 17);
            this.cB_refDrive.TabIndex = 10;
            this.cB_refDrive.Text = "perform reference drive";
            this.cB_refDrive.UseVisualStyleBackColor = true;
            // 
            // num_preCaptures
            // 
            this.num_preCaptures.Location = new System.Drawing.Point(377, 55);
            this.num_preCaptures.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.num_preCaptures.Name = "num_preCaptures";
            this.num_preCaptures.Size = new System.Drawing.Size(50, 20);
            this.num_preCaptures.TabIndex = 9;
            // 
            // num_MotorCOM
            // 
            this.num_MotorCOM.Location = new System.Drawing.Point(139, 55);
            this.num_MotorCOM.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.num_MotorCOM.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_MotorCOM.Name = "num_MotorCOM";
            this.num_MotorCOM.Size = new System.Drawing.Size(50, 20);
            this.num_MotorCOM.TabIndex = 7;
            this.num_MotorCOM.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // cB_Module
            // 
            this.cB_Module.FormattingEnabled = true;
            this.cB_Module.Items.AddRange(new object[] {
            "Tank Gauging",
            "RRC01",
            "Fractional-N PLL",
            "BGT24",
            "RRC160 DDS",
            "RRC160 Frac-N",
            "FindMine"});
            this.cB_Module.Location = new System.Drawing.Point(306, 13);
            this.cB_Module.Name = "cB_Module";
            this.cB_Module.Size = new System.Drawing.Size(121, 21);
            this.cB_Module.TabIndex = 6;
            // 
            // cB_Track
            // 
            this.cB_Track.FormattingEnabled = true;
            this.cB_Track.Items.AddRange(new object[] {
            "Millimeter Track",
            "FEW Lab",
            "Uni Ulm",
            "Simulation"});
            this.cB_Track.Location = new System.Drawing.Point(101, 13);
            this.cB_Track.Name = "cB_Track";
            this.cB_Track.Size = new System.Drawing.Size(121, 21);
            this.cB_Track.TabIndex = 5;
            this.cB_Track.SelectedIndexChanged += new System.EventHandler(this.cB_Track_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(255, 57);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(119, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Amount of PreCaptures:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(33, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Motor Control COM:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(255, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Module:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(57, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Track:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.num_Steps);
            this.tabPage2.Controls.Add(this.num_MultiAmount);
            this.tabPage2.Controls.Add(this.num_Stop);
            this.tabPage2.Controls.Add(this.num_MultiSteps);
            this.tabPage2.Controls.Add(this.num_Start);
            this.tabPage2.Controls.Add(this.cB_LoadConfig);
            this.tabPage2.Controls.Add(this.btn_SelConfigFile);
            this.tabPage2.Controls.Add(this.btn_selOutFolder);
            this.tabPage2.Controls.Add(this.tB_ConfigFile);
            this.tabPage2.Controls.Add(this.tB_OutFolder);
            this.tabPage2.Controls.Add(this.label12);
            this.tabPage2.Controls.Add(this.label13);
            this.tabPage2.Controls.Add(this.label11);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.btn_addTask);
            this.tabPage2.Controls.Add(this.lbl_TaskCount);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(537, 134);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Tasks";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // num_Steps
            // 
            this.num_Steps.Location = new System.Drawing.Point(470, 86);
            this.num_Steps.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.num_Steps.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_Steps.Name = "num_Steps";
            this.num_Steps.Size = new System.Drawing.Size(61, 20);
            this.num_Steps.TabIndex = 19;
            this.num_Steps.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // num_MultiAmount
            // 
            this.num_MultiAmount.Location = new System.Drawing.Point(310, 111);
            this.num_MultiAmount.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.num_MultiAmount.Name = "num_MultiAmount";
            this.num_MultiAmount.Size = new System.Drawing.Size(61, 20);
            this.num_MultiAmount.TabIndex = 18;
            this.num_MultiAmount.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // num_Stop
            // 
            this.num_Stop.Location = new System.Drawing.Point(310, 86);
            this.num_Stop.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.num_Stop.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_Stop.Name = "num_Stop";
            this.num_Stop.Size = new System.Drawing.Size(61, 20);
            this.num_Stop.TabIndex = 17;
            this.num_Stop.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // num_MultiSteps
            // 
            this.num_MultiSteps.Location = new System.Drawing.Point(119, 111);
            this.num_MultiSteps.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.num_MultiSteps.Name = "num_MultiSteps";
            this.num_MultiSteps.Size = new System.Drawing.Size(61, 20);
            this.num_MultiSteps.TabIndex = 16;
            this.num_MultiSteps.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // num_Start
            // 
            this.num_Start.Location = new System.Drawing.Point(119, 86);
            this.num_Start.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.num_Start.Name = "num_Start";
            this.num_Start.Size = new System.Drawing.Size(61, 20);
            this.num_Start.TabIndex = 15;
            // 
            // cB_LoadConfig
            // 
            this.cB_LoadConfig.AutoSize = true;
            this.cB_LoadConfig.Location = new System.Drawing.Point(149, 7);
            this.cB_LoadConfig.Name = "cB_LoadConfig";
            this.cB_LoadConfig.Size = new System.Drawing.Size(110, 17);
            this.cB_LoadConfig.TabIndex = 14;
            this.cB_LoadConfig.Text = "load configuration";
            this.cB_LoadConfig.UseVisualStyleBackColor = true;
            this.cB_LoadConfig.CheckedChanged += new System.EventHandler(this.cB_LoadConfig_CheckedChanged);
            // 
            // btn_SelConfigFile
            // 
            this.btn_SelConfigFile.Location = new System.Drawing.Point(451, 53);
            this.btn_SelConfigFile.Name = "btn_SelConfigFile";
            this.btn_SelConfigFile.Size = new System.Drawing.Size(75, 23);
            this.btn_SelConfigFile.TabIndex = 13;
            this.btn_SelConfigFile.Text = "select";
            this.btn_SelConfigFile.UseVisualStyleBackColor = true;
            this.btn_SelConfigFile.Click += new System.EventHandler(this.btn_SelConfigFile_Click);
            // 
            // btn_selOutFolder
            // 
            this.btn_selOutFolder.Location = new System.Drawing.Point(451, 28);
            this.btn_selOutFolder.Name = "btn_selOutFolder";
            this.btn_selOutFolder.Size = new System.Drawing.Size(75, 23);
            this.btn_selOutFolder.TabIndex = 12;
            this.btn_selOutFolder.Text = "select";
            this.btn_selOutFolder.UseVisualStyleBackColor = true;
            this.btn_selOutFolder.Click += new System.EventHandler(this.btn_selOutFolder_Click);
            // 
            // tB_ConfigFile
            // 
            this.tB_ConfigFile.Location = new System.Drawing.Point(90, 55);
            this.tB_ConfigFile.Name = "tB_ConfigFile";
            this.tB_ConfigFile.Size = new System.Drawing.Size(355, 20);
            this.tB_ConfigFile.TabIndex = 11;
            // 
            // tB_OutFolder
            // 
            this.tB_OutFolder.Location = new System.Drawing.Point(90, 30);
            this.tB_OutFolder.Name = "tB_OutFolder";
            this.tB_OutFolder.Size = new System.Drawing.Size(355, 20);
            this.tB_OutFolder.TabIndex = 10;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(201, 113);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(92, 13);
            this.label12.TabIndex = 9;
            this.label12.Text = "Multidata Amount:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(10, 113);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(108, 13);
            this.label13.TabIndex = 8;
            this.label13.Text = "Multidata Steps [mm]:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(391, 88);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(62, 13);
            this.label11.TabIndex = 7;
            this.label11.Text = "Steps [mm]:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(201, 88);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(102, 13);
            this.label10.TabIndex = 6;
            this.label10.Text = "Stop Distance [mm]:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 88);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(102, 13);
            this.label9.TabIndex = 5;
            this.label9.Text = "Start Distance [mm]:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 58);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 13);
            this.label8.TabIndex = 4;
            this.label8.Text = "Config File:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 33);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(74, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Output Folder:";
            // 
            // btn_addTask
            // 
            this.btn_addTask.Location = new System.Drawing.Point(452, 108);
            this.btn_addTask.Name = "btn_addTask";
            this.btn_addTask.Size = new System.Drawing.Size(75, 23);
            this.btn_addTask.TabIndex = 2;
            this.btn_addTask.Text = "add task";
            this.btn_addTask.UseVisualStyleBackColor = true;
            this.btn_addTask.Click += new System.EventHandler(this.btn_addTask_Click);
            // 
            // lbl_TaskCount
            // 
            this.lbl_TaskCount.AutoSize = true;
            this.lbl_TaskCount.Location = new System.Drawing.Point(103, 7);
            this.lbl_TaskCount.Name = "lbl_TaskCount";
            this.lbl_TaskCount.Size = new System.Drawing.Size(13, 13);
            this.lbl_TaskCount.TabIndex = 1;
            this.lbl_TaskCount.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 7);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(90, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Amount of Tasks:";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.num_isel_maxDist);
            this.tabPage3.Controls.Add(this.num_isel_stepsize);
            this.tabPage3.Controls.Add(this.label16);
            this.tabPage3.Controls.Add(this.label15);
            this.tabPage3.Controls.Add(this.label14);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(537, 134);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "isel-IMC4-M Settings";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // num_isel_maxDist
            // 
            this.num_isel_maxDist.Location = new System.Drawing.Point(171, 69);
            this.num_isel_maxDist.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.num_isel_maxDist.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_isel_maxDist.Name = "num_isel_maxDist";
            this.num_isel_maxDist.Size = new System.Drawing.Size(85, 20);
            this.num_isel_maxDist.TabIndex = 19;
            this.num_isel_maxDist.Value = new decimal(new int[] {
            2688,
            0,
            0,
            0});
            // 
            // num_isel_stepsize
            // 
            this.num_isel_stepsize.DecimalPlaces = 3;
            this.num_isel_stepsize.Location = new System.Drawing.Point(171, 44);
            this.num_isel_stepsize.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.num_isel_stepsize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_isel_stepsize.Name = "num_isel_stepsize";
            this.num_isel_stepsize.Size = new System.Drawing.Size(85, 20);
            this.num_isel_stepsize.TabIndex = 18;
            this.num_isel_stepsize.Value = new decimal(new int[] {
            9516,
            0,
            0,
            196608});
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(19, 71);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(127, 13);
            this.label16.TabIndex = 3;
            this.label16.Text = "Maximum distance in mm:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(88, 46);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(58, 13);
            this.label15.TabIndex = 2;
            this.label15.Text = "Steps/mm:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(19, 14);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(315, 13);
            this.label14.TabIndex = 1;
            this.label14.Text = "Please enter additional data for isel-IMC4-M Stepper Control Card:";
            // 
            // btn_nextCreate
            // 
            this.btn_nextCreate.Location = new System.Drawing.Point(242, 172);
            this.btn_nextCreate.Name = "btn_nextCreate";
            this.btn_nextCreate.Size = new System.Drawing.Size(75, 23);
            this.btn_nextCreate.TabIndex = 1;
            this.btn_nextCreate.Text = "next";
            this.btn_nextCreate.UseVisualStyleBackColor = true;
            this.btn_nextCreate.Click += new System.EventHandler(this.btn_nextCreate_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "Radar Automation Script-File|*.rasf";
            this.saveFileDialog1.RestoreDirectory = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.CheckPathExists = false;
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "RadarConfig-Files|*.rcfg";
            this.openFileDialog1.RestoreDirectory = true;
            // 
            // AutomationScriptCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(558, 206);
            this.Controls.Add(this.btn_nextCreate);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AutomationScriptCreator";
            this.Text = "AutomationScriptCreator";
            this.Load += new System.EventHandler(this.AutomationScriptCreator_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.pnl_PosTracker.ResumeLayout(false);
            this.pnl_PosTracker.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_TrackerCOM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_preCaptures)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_MotorCOM)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_Steps)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_MultiAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Stop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_MultiSteps)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Start)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_isel_maxDist)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_isel_stepsize)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ComboBox cB_Module;
        private System.Windows.Forms.ComboBox cB_Track;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btn_nextCreate;
        private System.Windows.Forms.NumericUpDown num_preCaptures;
        private System.Windows.Forms.NumericUpDown num_TrackerCOM;
        private System.Windows.Forms.NumericUpDown num_MotorCOM;
        private System.Windows.Forms.Button btn_addTask;
        private System.Windows.Forms.Label lbl_TaskCount;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox cB_refDrive;
        private System.Windows.Forms.CheckBox cB_LoadConfig;
        private System.Windows.Forms.Button btn_SelConfigFile;
        private System.Windows.Forms.Button btn_selOutFolder;
        private System.Windows.Forms.TextBox tB_ConfigFile;
        private System.Windows.Forms.TextBox tB_OutFolder;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown num_Steps;
        private System.Windows.Forms.NumericUpDown num_MultiAmount;
        private System.Windows.Forms.NumericUpDown num_Stop;
        private System.Windows.Forms.NumericUpDown num_MultiSteps;
        private System.Windows.Forms.NumericUpDown num_Start;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.NumericUpDown num_isel_maxDist;
        private System.Windows.Forms.NumericUpDown num_isel_stepsize;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Panel pnl_PosTracker;
    }
}