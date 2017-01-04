using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Radar_Config_and_Measurement_Tool
{
    public partial class NumericInputDialog : Form
    {
        public NumericInputDialog()
        {
            InitializeComponent();
            this.btn_Save.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {

        }
    }
}
