using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Radar_Config_and_Measurement_Tool
{
    public partial class Main : Form
    {
        private void btn_initAD5930_Click(object sender, EventArgs e)
        {
            UInt16 control = Convert.ToUInt16(lbl_AD5930_Control.Text, 16);
            UInt16 f_start_lsb = Convert.ToUInt16(lbl_AD5930_FSTART_lsb.Text, 16);
            UInt16 f_start_msb = Convert.ToUInt16(lbl_AD5930_FSTART_msb.Text, 16);
            UInt16 delta_f_lsb = Convert.ToUInt16(lbl_AD5930_DeltaF_lsb.Text, 16);
            UInt16 delta_f_msb = Convert.ToUInt16(lbl_AD5930_DeltaF_msb.Text, 16);
            UInt16 ninc = Convert.ToUInt16(lbl_AD5930_Ninc.Text, 16);
            UInt16 tINT = Convert.ToUInt16(lbl_AD5930_tINT.Text, 16);
            UInt16 tburst = Convert.ToUInt16(lbl_AD5930_TBURST.Text, 16);

            if (!com.openCOM(SettingsCollector.DSP_Com))
            {
                MessageBox.Show("Can't open " + SettingsCollector.DSP_Com + "!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!com.AD5930_DDS_Registers(control, f_start_lsb, f_start_msb, delta_f_lsb, delta_f_msb, ninc, tINT, tburst))
            {
                MessageBox.Show("Couldn't initialize DDS!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            com.closeCOM();

            if (e != null)
            {
                MessageBox.Show("Initialization successful", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void AD5930_ValueChanged(object sender, EventArgs e)
        {
            //Control
            UInt32 valueC = 3;
            for (int i = 2; i <= 11; i++)
            {
                string objectName = "cB_AD5930_Config_" + i.ToString("00");
                CheckBox cB = this.Controls.Find(objectName, true).FirstOrDefault() as CheckBox;
                if (cB.Checked)
                    valueC += (UInt32)Math.Pow(2, i);
            }

            lbl_AD5930_Control.Text = "0x" + valueC.ToString("X4");

            //Ninc
            UInt16 ninc = (UInt16)(num_AD5930_Ninc.Value + 4096); //Addr: 0001
            lbl_AD5930_Ninc.Text = "0x" + ninc.ToString("X4");

            //FSTART
            UInt32 fstart_24 = (UInt32)(num_AD5930_startF.Value);

            UInt32 fstart_lsb = ((fstart_24 & 0xFFF) + 49152); //Addr: 1100
            UInt32 fstart_msb = (((fstart_24 >> 12) & 0xFFF) + 53248); //Addr: 1101

            lbl_AD5930_FSTART_lsb.Text = "0x" + fstart_lsb.ToString("X4");
            lbl_AD5930_FSTART_msb.Text = "0x" + fstart_msb.ToString("X4");

            //DeltaF
            UInt32 deltaF_23 = (UInt32)(num_AD5930_deltaF.Value);

            UInt32 deltaF_lsb = ((deltaF_23 & 0xFFF) + 8192); //Addr: 0010
            UInt32 deltaF_msb = (((deltaF_23 >> 12) & 0x7FF) + 12288); //Addr: 0011

            if (cB_AD5930_DeltaF_negative.Checked)
                deltaF_msb += 2048; //Bit 11

            lbl_AD5930_DeltaF_lsb.Text = "0x" + deltaF_lsb.ToString("X4");
            lbl_AD5930_DeltaF_msb.Text = "0x" + deltaF_msb.ToString("X4");

            //tINT
            UInt16 tINT = (UInt16)(num_AD5930_tINT.Value + 16384); //Addr: 01xx
            if (cB_AD5930_tINT_mclk.Checked)
                tINT += 8192;
            tINT += (UInt16)(cB_AD5930_tINT_Multiplier.SelectedIndex * 2048);
            lbl_AD5930_tINT.Text = "0x" + tINT.ToString("X4");

            //TBURST
            UInt16 TBURST = (UInt16)(num_AD5930_TBURST.Value + 32768); //Addr: 10xx
            if (cB_AD5930_TBURST_mclk.Checked)
                TBURST += 8192;
            TBURST += (UInt16)(cB_AD5930_TBURST_Multiplier.SelectedIndex * 2048);
            lbl_AD5930_TBURST.Text = "0x" + TBURST.ToString("X4");
        }

        public void AD5930_setFromConfig(UInt16 DDS_Control, UInt16 DDS_fstart_lsb, UInt16 DDS_fstart_msb, UInt16 DDS_deltaF_lsb,
            UInt16 DDS_deltaF_msb, UInt16 DDS_Ninc, UInt16 DDS_tINT, UInt16 DDS_TBURST)
        {
            //control
            for (int i = 2; i <= 11; i++)
            {
                string objectName = "cB_AD5930_Config_" + i.ToString("00");
                CheckBox cB = this.Controls.Find(objectName, true).FirstOrDefault() as CheckBox;
                UInt32 mask = (UInt32)(1 << i);
                cB.Checked = ((DDS_Control & (mask)) == mask);
            }

            //FSTART
            num_AD5930_startF.Value = ((DDS_fstart_msb & 0xFFF) << 12) + (DDS_fstart_lsb & 0xFFF);

            //Delta f
            num_AD5930_deltaF.Value = ((DDS_deltaF_msb & 0x7FF) << 12) + (DDS_deltaF_lsb & 0xFFF);

            cB_AD5930_DeltaF_negative.Checked = ((DDS_deltaF_msb & 0x800) == 0x800);

            //Ninc
            num_AD5930_Ninc.Value = (DDS_Ninc & 0xFFF);

            //tInt
            num_AD5930_tINT.Value = (DDS_tINT & 0x7FF);
            cB_AD5930_tINT_mclk.Checked = ((DDS_tINT & 0x2000) == 0x2000);
            cB_AD5930_tINT_Multiplier.SelectedIndex = (DDS_tINT & 0x1800) >> 11;

            //TBURST
            num_AD5930_TBURST.Value = (DDS_TBURST & 0x7FF);
            cB_AD5930_TBURST_mclk.Checked = ((DDS_TBURST & 0x2000) == 0x2000);
            cB_AD5930_TBURST_Multiplier.SelectedIndex = (DDS_TBURST & 0x1800) >> 11;

        }
    }
}
