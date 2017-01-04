using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Radar_Config_and_Measurement_Tool
{
    public partial class Main : Form
    {
        private void ADF4108_ValueChanged(object sender, EventArgs e)
        {
            UInt32 valueAB = 1;
            UInt32 valueF = 2;
            UInt32 valueR = 0;
            for (int i = 2; i <= 23; i++)
            {
                string objectNameAB = "cB_ADF4108_AB_" + i.ToString("00");
                string objectNameF = "cB_ADF4108_F_" + i.ToString("00");
                string objectNameR = "cB_ADF4108_R_" + i.ToString("00");
                //AB
                if (i <= 21)
                {
                    CheckBox cB_AB = this.Controls.Find(objectNameAB, true).FirstOrDefault() as CheckBox;
                    if (cB_AB.Checked)
                        valueAB += (UInt32)Math.Pow(2, i); ;

                }
                //R
                if (i <= 20)
                {
                    CheckBox cB_R = this.Controls.Find(objectNameR, true).FirstOrDefault() as CheckBox;
                    if (cB_R.Checked)
                        valueR += (UInt32)Math.Pow(2, i); ;
                }
                //F
                CheckBox cB_F = this.Controls.Find(objectNameF, true).FirstOrDefault() as CheckBox;
                if (cB_F.Checked)
                    valueF += (UInt32)Math.Pow(2, i); ;
            }

            lbl_ADF4108_AB.Text = "0x" + valueAB.ToString("X6");
            lbl_ADF4108_Func.Text = "0x" + valueF.ToString("X6");
            lbl_ADF4108_R.Text = "0x" + valueR.ToString("X6");

            updateSysInfoBox(sender, e);
        }

        private void btn_initADF4108_Click(object sender, EventArgs e)
        {
            UInt32 f = Convert.ToUInt32(lbl_ADF4108_Func.Text, 16);
            UInt32 r = Convert.ToUInt32(lbl_ADF4108_R.Text, 16);
            UInt32 ab = Convert.ToUInt32(lbl_ADF4108_AB.Text, 16);

            if (!com.openCOM(SettingsCollector.DSP_Com))
            {
                MessageBox.Show("Can't open " + SettingsCollector.DSP_Com + "!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!com.ADF4108_PLL_Registers(f, r, ab))
            {
                MessageBox.Show("Couldn't initialize PLL!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            com.closeCOM();

            if (e != null)
            {
                MessageBox.Show("Initialization successful", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        public void ADF4108_setFromConfig(UInt32 PLL_ref, UInt32 PLL_ab, UInt32 PLL_func)
        {
            //AB (only CP gain)
            cB_ADF4108_AB_21.Checked = ((PLL_ab & 0x200000) == 0x200000);

            //ref
            for (int i = 2; i <= 20; i++)
            {
                string objectName = "cB_ADF4108_R_" + i.ToString("00");
                CheckBox cB = this.Controls.Find(objectName, true).FirstOrDefault() as CheckBox;
                UInt32 mask = (UInt32)(1 << i);
                cB.Checked = ((PLL_ref & (mask)) == mask);
            }
            //func
            for (int i = 2; i <= 21; i++)
            {
                string objectName = "cB_ADF4108_F_" + i.ToString("00");
                CheckBox cB = this.Controls.Find(objectName, true).FirstOrDefault() as CheckBox;
                UInt32 mask = (UInt32)(1 << i);
                cB.Checked = ((PLL_func & (mask)) == mask);
            }
        }
    }
}
