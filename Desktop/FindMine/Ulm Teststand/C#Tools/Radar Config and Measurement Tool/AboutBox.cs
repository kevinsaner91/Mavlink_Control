using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.IO;

namespace Radar_Config_and_Measurement_Tool
{
    partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
            this.Text = String.Format("About {0}", AssemblyTitle);
            this.labelProductName.Text = AssemblyProduct;

            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                Version v = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
                this.labelVersion.Text = "Version " + v.Major.ToString() + "." + v.Minor.ToString() + "." + v.Build.ToString();
            }
            else
            {
                Version v = Assembly.GetExecutingAssembly().GetName().Version;
                this.labelVersion.Text = "Portable Version (V" + v.Major + "." + v.Minor + "." + v.Build + ")";
            }
            this.labelCopyright.Text = AssemblyCopyright;
            this.labelDescription.Text = AssemblyCompany;
            this.textBoxDescription.Text = @"Changelog:
V1.5: (MSa) - 06.07.2016
 - Add/Fix: Multiple Window Types selectable

V1.4.1: (MSa) - 03.05.2016
 - Fix: Exception on Fractional N PLL board fixed

V1.4: (MSa) - 29.03.2016
 - Add: Module FindMine
 - Fix: System calculation for DDS Hardware when System Clock != DDS frequency
 - Fix: R counter of ADF4108 will now be included in calculations
 - Fix: Bug for Automation Script Creator maximum value incorrect for Uni Ulm track 
 - Change: Automation script will include given end position now
 - Change: Y Axis of spectrum chart in capture tab
 - Add: Show dB and m of maximum in capture tab

V1.3.3: (MSa) - 12.02.2016
 - Add: Debug output for track motor/laser
 - Add: Advanced -> Set Automation Waiting Time
 - Fix: Bugfix for Automation of Uni Ulm track (slow motor control)
 - Change: Include Matlab Evaluation into .exe File.

V1.3.2: (MSa) - 02.02.2016
 - Change: CSV Filenames changed from 3 to 6 digits
 - Add: Option to enable/disable *.png capture

V1.3.1: (MSa) - 26.01.2016
 - Fix: Bugfixes for Uni Ulm track

V1.3: (MSa) - 25.01.2016
 - Change: save to csv in Capture tab for a given time/amount or until user stops
 - Add: optional waiting time between cycles in capture mode
 - Change: timestamp in csv file 
 - Add: Uni Ulm track
 - Fix: Minor bugfixes

V1.2.2: (MSa) - 19.01.2016
 - Fix: Load config on startup may throw exception

V1.2.1: (MSa) - 15.01.2016
 - Fix: bugfixes for calculation of RRC160 boards and Fractional-N boards
 - Changed: Renamed RRC160 DDS to RRC160 Integer-N
 - Changed: Default setup for RRC160 Boards

V1.2: (MSa) - 01.12.2015
 - Add: Modules RRC160 DDS and RRC160 FracN
 - Add: Possibility to simulate Measurement Track in automation scripts
 - Add: Save Automation Log to File (to save into Cloud Service for external monitoring of task) 
 - Changed: Merged InfoBox of Frac N modules and DDS modules
 - Fix: several minor bugfixes

V1.1.1: (MSa) - 23.10.2015
 - Add: Advanced -> Get Module Information
 - Add: Check for correct module before initializing

V1.1: (MSa) - 29.09.2015
 - Fix: Millimeter Track Control
 - Fix: Errors with Frac-N PLL
 - Updated Frac-N Default Setup
 - Changed: Precaptures of Automation will be performed at first position
 - Add: Matlab Linearity Evaluation

V1.0.2: (MSa) - 23.09.2015
 - Fix: Default Configuration and Matlab Evaluation

V1.0.1: (MSa) - 14.09.2015
 - Add: Capture Charts are zoomable
 - Add: Tooltips with distance and amplitude information for FFT points in capture tab
 - Fix: Matlab evaluation produced error messages

V1.0: (MSa) - 14.09.2015
 - Add: BGT24MTR11 (Eval Board B) support
 - Add: Possibility to load default configuration on board selection
 - Fix: Ramp selection of Capture screen

V0.9 Beta 2: (MSa) - 24.06.2015
 - Fix: Errors occured by cultural settings of pc
 - Add: Matlab evaluation
 - Add: Creation of data.m for Matlab evaluation 

V0.9 Beta 1: (MSa) - 19.06.2015
 - First release for TankGauging, RRC01 and Fractional-N

V0.1: (MSa) - 04.05.2015
 - Initial Release";
        }

        #region Assemblyattributaccessoren

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
