using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace Radar_Config_and_Measurement_Tool
{
    public partial class DSP_ComSelect : Form
    {        
        public DSP_ComSelect()
        {
            InitializeComponent();

            string[] ports = SerialPort.GetPortNames();

            Array.Sort(ports, StringComparer.InvariantCulture); 

            cB_Com.Items.Clear();

            foreach (string p in ports)
            {
                cB_Com.Items.Add(p);
            }

            int idx = cB_Com.Items.IndexOf(SettingsCollector.DSP_Com);
            if (idx >= 0)
            {
                cB_Com.SelectedIndex = idx;
            }
            else
            {
                cB_Com.SelectedIndex = 0;
            }
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            try
            {
                SettingsCollector.DSP_Com = cB_Com.SelectedItem.ToString();
            }
            catch (Exception)
            {
                MessageBox.Show("Selected COM-Port not possible!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Close();
            }
        }
    }
}
