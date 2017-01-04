using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Radar_Config_and_Measurement_Tool
{
    public partial class Main : Form
    {
        private void HMC_ValueChanged(object sender, EventArgs e)
        {
            //01: RST
            UInt32 valueRST = 1 * (UInt32)Math.Pow(2, 24);
            for (int i = 0; i <= 10; i++)
            {
                string objectName = "cB_HMC_RST_" + i.ToString("00");
                CheckBox cB = this.Controls.Find(objectName, true).FirstOrDefault() as CheckBox;
                if (cB.Checked)
                    valueRST += (UInt32)Math.Pow(2, i);
            }
            lbl_HMC_RST.Text = "0x" + valueRST.ToString("X8");

            //02: REFDIV
            UInt32 valueREFDIV = 2 * (UInt32)Math.Pow(2, 24) + (UInt32)num_HMC_RefDiv.Value;
            lbl_HMC_REFDIV.Text = "0x" + valueREFDIV.ToString("X8");

            //03: Freq. (int)
            UInt32 valueFInt = 3 * (UInt32)Math.Pow(2, 24) + (UInt32)num_HMC_FRegInt.Value;
            lbl_HMC_FRegInt.Text = "0x" + valueFInt.ToString("X8");

            //04: Freq. (frac)
            UInt32 valueFFrac = 4 * (UInt32)Math.Pow(2, 24) + (UInt32)num_HMC_FRegFrac.Value;
            lbl_HMC_FRegFrac.Text = "0x" + valueFFrac.ToString("X8");

            //05: Seed
            UInt32 valueSeed = 5 * (UInt32)Math.Pow(2, 24) + (UInt32)num_HMC_Seed.Value;
            lbl_HMC_Seed.Text = "0x" + valueSeed.ToString("X8");

            //06: SD CFG
            UInt32 valueSD = 6 * (UInt32)Math.Pow(2, 24) + 15 * (UInt32)Math.Pow(2, 1) + 7 * (UInt32)Math.Pow(2, 10);
            valueSD += (UInt32)(cB_HMC_ModType.SelectedIndex + cB_HMC_SDMode.SelectedIndex * Math.Pow(2, 5) + cB_HMC_DSM_source.SelectedIndex * Math.Pow(2, 17));
            if (cB_HMC_autoseed.Checked)
                valueSD += (UInt32)(Math.Pow(2, 8));
            if (cB_HMC_extTrig.Checked)
                valueSD += (UInt32)(Math.Pow(2, 9));
            if (cB_HMC_forceDSM.Checked)
                valueSD += (UInt32)(Math.Pow(2, 13));
            if (cB_HMC_forceRDIV.Checked)
                valueSD += (UInt32)(Math.Pow(2, 21));
            if (cB_HMC_disExtACCReset.Checked)
                valueSD += (UInt32)(Math.Pow(2, 22));
            if (cB_HMC_singleStepRamp.Checked)
                valueSD += (UInt32)(Math.Pow(2, 23));
            lbl_HMC_SdCfg.Text = "0x" + valueSD.ToString("X8");

            //07: LockDetect
            UInt32 valueLD = 7 * (UInt32)Math.Pow(2, 24) + 12 * (UInt32)Math.Pow(2, 3);
            valueLD += (UInt32)(cB_HMC_LockDetect.SelectedIndex);
            if (cB_HMC_LD_Counters.Checked)
                valueLD += (UInt32)(Math.Pow(2, 11));
            if (cB_HMC_LD_Timer.Checked)
                valueLD += (UInt32)(Math.Pow(2, 14));
            if (cB_HMC_cycleslipPrevention.Checked)
                valueLD += (UInt32)(Math.Pow(2, 15));
            if (cB_HMC_trainLD.Checked)
                valueLD += (UInt32)(Math.Pow(2, 20));

            lbl_HMC_LockD.Text = "0x" + valueLD.ToString("X8");

            //08: AnalogEn
            UInt32 valueAnEn = 8 * (UInt32)Math.Pow(2, 24) + (UInt32)num_HMC_VCOOutBiasA.Value * (UInt32)Math.Pow(2, 10) + (UInt32)num_HMC_VCOOutBiasB.Value * (UInt32)Math.Pow(2, 13);
            int[] tmp = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 16, 17, 18, 19 };
            foreach (int i in tmp)
            {
                string objectName = "cB_HMC_AnEn_" + i.ToString("00");
                CheckBox cB = this.Controls.Find(objectName, true).FirstOrDefault() as CheckBox;
                if (cB.Checked)
                    valueAnEn += (UInt32)Math.Pow(2, i);
            }

            lbl_HMC_AnalogEn.Text = "0x" + valueAnEn.ToString("X8");

            //09: Charge Pump
            UInt32 valueCharge = 9 * (UInt32)Math.Pow(2, 24) + (UInt32)(num_HMC_mainSinkCurrent.Value / 20) + (UInt32)(num_HMC_mainSourceCurrent.Value / 20) * (UInt32)Math.Pow(2, 7) + (UInt32)(num_HMC_offsetCurrent.Value / 5) * (UInt32)Math.Pow(2, 14);
            if (cB_HMC_sourceOffPol.Checked)
                valueCharge += (UInt32)(Math.Pow(2, 21));
            if (cB_HMC_sinkOffPol.Checked)
                valueCharge += (UInt32)(Math.Pow(2, 22));
            if (cB_HMC_hiGainMode.Checked)
                valueCharge += (UInt32)(Math.Pow(2, 23));

            lbl_HMC_ChargeP.Text = "0x" + valueCharge.ToString("X8");

            //10: Modulation Step
            UInt32 valueMStep = 10 * (UInt32)Math.Pow(2, 24) + (UInt32)((Int32)num_HMC_ModStep.Value & 0xFFFFFF);
            lbl_HMC_ModStep.Text = "0x" + valueMStep.ToString("X8");

            //11: PD
            UInt32 valuePD = 11 * (UInt32)Math.Pow(2, 24) + (UInt32)num_HMC_PD_deadZone.Value + (UInt32)num_HMC_PD_PSBias.Value * (UInt32)Math.Pow(2, 10)
                + (UInt32)num_HMC_PD_OPBias.Value * (UInt32)Math.Pow(2, 13) + (UInt32)num_HMC_PD_Mcnt.Value * (UInt32)Math.Pow(2, 15);
            int[] tmpPD = { 3, 4, 5, 6, 7, 8, 9, 17, 18 };
            foreach (int i in tmpPD)
            {
                string objectName = "cB_HMC_PD_" + i.ToString("00");
                CheckBox cB = this.Controls.Find(objectName, true).FirstOrDefault() as CheckBox;
                if (cB.Checked)
                    valuePD += (UInt32)Math.Pow(2, i);
            }

            lbl_HMC_PD.Text = "0x" + valuePD.ToString("X8");

            //12: ALTINT
            UInt32 valueAI = 12 * (UInt32)Math.Pow(2, 24) + (UInt32)num_HMC_ALTINT.Value;
            lbl_HMC_ALTINT.Text = "0x" + valueAI.ToString("X8");

            //13: ALTFRAC
            UInt32 valueAF = 13 * (UInt32)Math.Pow(2, 24) + (UInt32)num_HMC_ALTFRAC.Value;
            lbl_HMC_ALTFRAC.Text = "0x" + valueAF.ToString("X8");

            //15: GPO
            UInt32 valueGPO = 15 * (UInt32)Math.Pow(2, 24) + (UInt32)cB_HMC_muxout.SelectedIndex;
            for (int i = 5; i <= 9; i++)
            {
                string objectName = "cB_HMC_GPO_" + i.ToString("00");
                CheckBox cB = this.Controls.Find(objectName, true).FirstOrDefault() as CheckBox;
                if (cB.Checked)
                    valueGPO += (UInt32)Math.Pow(2, i);
            }

            lbl_HMC_GPO.Text = "0x" + valueGPO.ToString("X8");

            updateSysInfoBox(sender, e); //to update REFDIV and "Enable RF divide/2"
        }

        private void btn_initHMC_Click(object sender, EventArgs e)
        {
            UInt32 r00 = Convert.ToUInt32(lbl_HMC_ID.Text, 16);
            UInt32 r01 = Convert.ToUInt32(lbl_HMC_RST.Text, 16);
            UInt32 r02 = Convert.ToUInt32(lbl_HMC_REFDIV.Text, 16);
            UInt32 r03 = Convert.ToUInt32(lbl_HMC_FRegInt.Text, 16);
            UInt32 r04 = Convert.ToUInt32(lbl_HMC_FRegFrac.Text, 16);
            UInt32 r05 = Convert.ToUInt32(lbl_HMC_Seed.Text, 16);
            UInt32 r06 = Convert.ToUInt32(lbl_HMC_SdCfg.Text, 16);
            UInt32 r07 = Convert.ToUInt32(lbl_HMC_LockD.Text, 16);
            UInt32 r08 = Convert.ToUInt32(lbl_HMC_AnalogEn.Text, 16);
            UInt32 r09 = Convert.ToUInt32(lbl_HMC_ChargeP.Text, 16);
            UInt32 r10 = Convert.ToUInt32(lbl_HMC_ModStep.Text, 16);
            UInt32 r11 = Convert.ToUInt32(lbl_HMC_PD.Text, 16);
            UInt32 r12 = Convert.ToUInt32(lbl_HMC_ALTINT.Text, 16);
            UInt32 r13 = Convert.ToUInt32(lbl_HMC_ALTFRAC.Text, 16);
            UInt32 r14 = Convert.ToUInt32(lbl_HMC_SPITrig.Text, 16);
            UInt32 r15 = Convert.ToUInt32(lbl_HMC_GPO.Text, 16);

            if (!com.openCOM(SettingsCollector.DSP_Com))
            {
                MessageBox.Show("Can't open " + SettingsCollector.DSP_Com + "!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!com.HMC703_Registers(r00, r01, r02, r03, r04, r05, r06, r07, r08, r09, r10, r11, r12, r13, r14, r15))
            {
                MessageBox.Show("Couldn't initialize DDS!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            com.closeCOM();

            if (e != null)
            {
                MessageBox.Show("Initialization successful", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        public void HMC703_setFromConfig(UInt32 r01, UInt32 r02, UInt32 r03, UInt32 r04, UInt32 r05, UInt32 r06, 
            UInt32 r07, UInt32 r08, UInt32 r09, UInt32 r10, UInt32 r11, UInt32 r12, UInt32 r13, UInt32 r15 )
        {
            //01: RST
            for (int i = 0; i <= 10; i++)
            {
                string objectName = "cB_HMC_RST_" + i.ToString("00");
                CheckBox cB = this.Controls.Find(objectName, true).FirstOrDefault() as CheckBox;
                UInt32 mask = (UInt32)(1 << i);
                cB.Checked = ((r01 & (mask)) == mask);
                   
            }

            //02: REFDIV
            num_HMC_RefDiv.Value = r02 & 0x3FFF;

            //03: Freq. (int)
            num_HMC_FRegInt.Value = r03 & 0xFFFF;

            //04: Freq. (frac)
            num_HMC_FRegFrac.Value = r04 & 0xFFFFFF;

            //05: Seed
            num_HMC_Seed.Value = r05 & 0xFFFFFF;

            //06: SD CFG
            cB_HMC_ModType.SelectedIndex = (int)(r06 & 0x1);
            cB_HMC_SDMode.SelectedIndex = (int)((r06 & 0xE0)>>5);
            cB_HMC_DSM_source.SelectedIndex = (int)((r06 & 0x60000) >> 17);
            cB_HMC_autoseed.Checked = ((r06 & (0x100)) == 0x100);
            cB_HMC_extTrig.Checked = ((r06 & (0x200)) == 0x200);
            cB_HMC_forceDSM.Checked = ((r06 & (0x2000)) == 0x2000);
            cB_HMC_forceRDIV.Checked = ((r06 & (0x200000)) == 0x200000);
            cB_HMC_disExtACCReset.Checked = ((r06 & (0x400000)) == 0x400000);
            cB_HMC_singleStepRamp.Checked = ((r06 & (0x800000)) == 0x800000);

            //07: LockDetect
            cB_HMC_LockDetect.SelectedIndex = (int)(r07 & 0x7);
            cB_HMC_LD_Counters.Checked = ((r07 & (0x800)) == 0x800);
            cB_HMC_LD_Timer.Checked = ((r07 & (0x4000)) == 0x4000);
            cB_HMC_cycleslipPrevention.Checked = ((r07 & (0x8000)) == 0x8000);
            cB_HMC_trainLD.Checked = ((r07 & (0x100000)) == 0x100000);

            //08: AnalogEn
            num_HMC_VCOOutBiasA.Value = (r08 & 0x1C00)>>10;
            num_HMC_VCOOutBiasB.Value = (r08 & 0xE000) >> 13;
            int[] tmp = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 16, 17, 18, 19 };
            foreach (int i in tmp)
            {
                string objectName = "cB_HMC_AnEn_" + i.ToString("00");
                CheckBox cB = this.Controls.Find(objectName, true).FirstOrDefault() as CheckBox;
                UInt32 mask = (UInt32)(1 << i);
                cB.Checked = ((r08 & (mask)) == mask);
            }

            //09: Charge Pump
            num_HMC_mainSinkCurrent.Value = (r09 & 0x7F) * 20;
            num_HMC_mainSourceCurrent.Value = ((r09 & 0x3F80)>>7) * 20;
            num_HMC_offsetCurrent.Value = ((r09 & 0x1FC000)>>14) * 5;
            cB_HMC_sourceOffPol.Checked = ((r09 & (0x200000)) == 0x200000);
            cB_HMC_sinkOffPol.Checked = ((r09 & (0x400000)) == 0x400000);
            cB_HMC_hiGainMode.Checked = ((r09 & (0x800000)) == 0x800000);

            //10: Modulation Step
            UInt32 tmp_m = r10 & 0xFFFFFF;
            if ((tmp_m & 0x800000) == 0x800000)
            {
                tmp_m |= 0xFF000000;
                tmp_m = ~tmp_m;
                tmp_m++;
                num_HMC_ModStep.Value = -1 * (Int32)tmp_m;
            }
            else
            {
                num_HMC_ModStep.Value = tmp_m;
            }

            //11: PD
            num_HMC_PD_deadZone.Value = (r11 & 0x7);
            num_HMC_PD_PSBias.Value = ((r11 & 0x1C00)>>10);
            num_HMC_PD_OPBias.Value = ((r11 & 0x6000)>>13);
            num_HMC_PD_Mcnt.Value = ((r11 & 0x18000) >> 15);

            int[] tmpPD = { 3, 4, 5, 6, 7, 8, 9, 17, 18 };
            foreach (int i in tmpPD)
            {
                string objectName = "cB_HMC_PD_" + i.ToString("00");
                CheckBox cB = this.Controls.Find(objectName, true).FirstOrDefault() as CheckBox;
                UInt32 mask = (UInt32)(1 << i);
                cB.Checked = ((r11 & (mask)) == mask);
            }

            //12: ALTINT
            num_HMC_ALTINT.Value = r12 & 0xFFFF;

            //13: ALTFRAC
            num_HMC_ALTFRAC.Value = r13 & 0xFFFFFF;

            //15: GPO
            cB_HMC_muxout.SelectedIndex = (int)(r15 & 0x1F);
            for (int i = 5; i <= 9; i++)
            {
                string objectName = "cB_HMC_GPO_" + i.ToString("00");
                CheckBox cB = this.Controls.Find(objectName, true).FirstOrDefault() as CheckBox;
                UInt32 mask = (UInt32)(1 << i);
                cB.Checked = ((r15 & (mask)) == mask);
            }
        }
    }
}
