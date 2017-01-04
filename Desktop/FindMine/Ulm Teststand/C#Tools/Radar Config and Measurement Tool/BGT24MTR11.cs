using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Radar_Config_and_Measurement_Tool
{
    public partial class Main : Form
    {
        private void BGT24_ValueChanged(object sender, EventArgs e)
        {
            UInt32 value = (UInt32)cB_BGT_TX_Atten.SelectedIndex;

            value += (UInt32)(cB_BGT_LNAgain.SelectedIndex * 32768);

            value += (UInt32)((cB_BGT_AnMUX.SelectedIndex & 0x3) * 128);
            if (cB_BGT_AnMUX.SelectedIndex >= 4)
                value += 2048;

            if (cB_BGT_HighTX.Checked)
                value += 8;

            if (cB_BGT_HighLO.Checked)
                value += 16;

            if (cB_BGT_disable16Div.Checked)
                value += 32;

            if (cB_BGT_disable64kDiv.Checked)
                value += 64;

            if (cB_BGT_disableTX.Checked)
                value += 4096;

            lbl_BGT_control.Text = "0x" + value.ToString("X4");
        }
        
        private void btn_initBGT24_Click(object sender, EventArgs e)
        {
            UInt16 control = Convert.ToUInt16(lbl_BGT_control.Text, 16);

            if (!com.openCOM(SettingsCollector.DSP_Com))
            {
                MessageBox.Show("Can't open " + SettingsCollector.DSP_Com + "!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!com.BGT24MTR11_Control(control))
            {
                MessageBox.Show("Couldn't initialize BGT24MTR11!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            com.closeCOM();

            MessageBox.Show("Initialization successful", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        public void BGT24_setFromConfig(UInt32 control)
        {
            cB_BGT_TX_Atten.SelectedIndex = (int)(control & 0x7);
            
            if ((control & 0x8) > 0)
                cB_BGT_HighTX.Checked = true;
            else
                cB_BGT_HighTX.Checked = false;

            if ((control & 0x10) > 0)
                cB_BGT_HighLO.Checked = true;
            else
                cB_BGT_HighLO.Checked = false;

            if ((control & 0x20) > 0)
                cB_BGT_disable16Div.Checked = true;
            else
                cB_BGT_disable16Div.Checked = false;

            if ((control & 0x40) > 0)
                cB_BGT_disable64kDiv.Checked = true;
            else
                cB_BGT_disable64kDiv.Checked = false;

            cB_BGT_AnMUX.SelectedIndex = (int)((((control & 0x800) >> 2) + (control & 0x180))>>7);

            if ((control & 0x1000) > 0)
                cB_BGT_disableTX.Checked = true;
            else
                cB_BGT_disableTX.Checked = false;

            cB_BGT_LNAgain.SelectedIndex = (int)((control & 0x8000) >> 15);
        }

        private void btn_BGT_Mon_updateTX_Click(object sender, EventArgs e)
        {
            UInt16 controlSave;

            UInt16 channel0, channel1;
            float valueMux0, valueMux1, TXpower;

            UInt16 control = controlSave = Convert.ToUInt16(lbl_BGT_control.Text, 16);

            if (!com.openCOM(SettingsCollector.DSP_Com))
            {
                MessageBox.Show("Can't open " + SettingsCollector.DSP_Com + "!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            control &= 0xF67F; //set control 0
            control |= 0x80; //set control 1
            if (!com.BGT24MTR11_Control(control))
            {
                MessageBox.Show("Couldn't initialize BGT24MTR11!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                com.closeCOM();
                return;
            }
            if (!com.readBGT24AuxADC(out channel0, out channel1))
            {
                MessageBox.Show("Couldn't read BGT24 Aux ADC!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                com.closeCOM();
                return;
            }
            valueMux1 = channel1 / 4096.0f * 3.0f;

            control &= 0xF67F; //set control 0
            if (!com.BGT24MTR11_Control(control))
            {
                MessageBox.Show("Couldn't initialize BGT24MTR11!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                com.closeCOM();
                return;
            }
            if (!com.readBGT24AuxADC(out channel0, out channel1))
            {
                MessageBox.Show("Couldn't read BGT24 Aux ADC!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                com.closeCOM();
                return;
            }
            valueMux0 = channel1 / 4096.0f * 3.0f;

            //restore BGT24 control
            if (!com.BGT24MTR11_Control(controlSave))
            {
                MessageBox.Show("Couldn't initialize BGT24MTR11!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                com.closeCOM();
                return;
            }

            com.closeCOM();

            TXpower = (valueMux0 - valueMux1) * 1000;
            tB_BGT_Mon_TX.Text = TXpower.ToString("#.#");
        }

        private void btn_BGT_Mon_updateLO_Click(object sender, EventArgs e)
        {
            UInt16 controlSave;

            UInt16 channel0, channel1;
            float valueMux0, valueMux1, TXpower;

            UInt16 control = controlSave = Convert.ToUInt16(lbl_BGT_control.Text, 16);

            if (!com.openCOM(SettingsCollector.DSP_Com))
            {
                MessageBox.Show("Can't open " + SettingsCollector.DSP_Com + "!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            control &= 0xF67F; //set control 0
            control |= 0x180; //set control 3
            if (!com.BGT24MTR11_Control(control))
            {
                MessageBox.Show("Couldn't initialize BGT24MTR11!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                com.closeCOM();
                return;
            }
            if (!com.readBGT24AuxADC(out channel0, out channel1))
            {
                MessageBox.Show("Couldn't read BGT24 Aux ADC!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                com.closeCOM();
                return;
            }
            valueMux1 = channel1 / 4096.0f * 3.0f;

            control &= 0xF67F; //set control 0
            control |= 0x100; //set control 2
            if (!com.BGT24MTR11_Control(control))
            {
                MessageBox.Show("Couldn't initialize BGT24MTR11!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                com.closeCOM();
                return;
            }
            if (!com.readBGT24AuxADC(out channel0, out channel1))
            {
                MessageBox.Show("Couldn't read BGT24 Aux ADC!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                com.closeCOM();
                return;
            }
            valueMux0 = channel1 / 4096.0f * 3.0f;

            //restore BGT24 control
            if (!com.BGT24MTR11_Control(controlSave))
            {
                MessageBox.Show("Couldn't initialize BGT24MTR11!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                com.closeCOM();
                return;
            }

            com.closeCOM();

            TXpower = (valueMux0 - valueMux1) * 1000;
            tB_BGT_Mon_LO.Text = TXpower.ToString("#.#");
        }

        private void btn_BGT_Mon_updateTemp_Click(object sender, EventArgs e)
        {
            UInt16 controlSave;

            UInt16 channel0, channel1;
            float temperature;

            UInt16 control = controlSave = Convert.ToUInt16(lbl_BGT_control.Text, 16);

            if (!com.openCOM(SettingsCollector.DSP_Com))
            {
                MessageBox.Show("Can't open " + SettingsCollector.DSP_Com + "!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            control &= 0xF67F; //set control 0
            control |= 0x800; //set control 4
            if (!com.BGT24MTR11_Control(control))
            {
                MessageBox.Show("Couldn't initialize BGT24MTR11!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                com.closeCOM();
                return;
            }
            if (!com.readBGT24AuxADC(out channel0, out channel1))
            {
                MessageBox.Show("Couldn't read BGT24 Aux ADC!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                com.closeCOM();
                return;
            }

            temperature = ((channel1 / 4096.0f * 3.0f) - 1.55f) / 0.004f + 25;

            //restore BGT24 control
            if (!com.BGT24MTR11_Control(controlSave))
            {
                MessageBox.Show("Couldn't initialize BGT24MTR11!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                com.closeCOM();
                return;
            }

            com.closeCOM();
            tB_BGT_Mon_Temp.Text = temperature.ToString("#.#");
        }

        private void btn_BGT_Mon_updateRef25_Click(object sender, EventArgs e)
        {
            UInt16 channel0, channel1;
            float refVoltage;

            if (!com.openCOM(SettingsCollector.DSP_Com))
            {
                MessageBox.Show("Can't open " + SettingsCollector.DSP_Com + "!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!com.readBGT24AuxADC(out channel0, out channel1))
            {
                MessageBox.Show("Couldn't read BGT24 Aux ADC!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                com.closeCOM();
                return;
            }

            refVoltage = channel0 / 4096.0f * 3.0f;

            com.closeCOM();

            tB_BGT_Mon_Ref25.Text = refVoltage.ToString("#.##");
        }
    }
}
