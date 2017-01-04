using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Radar_Config_and_Measurement_Tool
{
    public partial class Main : Form
    {
        private UInt16 windowIdx = 0;
        private UInt16 windowSize = 0;
        private float[] windowParam = { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private enum WindowType { Rectangular = 0, Chebyshev80, VonHann, Hamming, Gauss, BlackmannHarris, SelfDefined };

        private void updateWindowParameter(object sender, EventArgs e)
        {
            windowIdx = (UInt16)cB_Sys_window.SelectedIndex;
            switch (windowIdx)
            {
                case (int)WindowType.Rectangular:
                    windowSize = 0;
                    windowParam[0] = 1.0f;
                    windowParam[1] = 0.0f;
                    windowParam[2] = 0.0f;
                    windowParam[3] = 0.0f;
                    windowParam[4] = 0.0f;
                    windowParam[5] = 0.0f;
                    windowParam[6] = 0.0f;
                    windowParam[7] = 0.0f;
                    windowParam[8] = 0.0f;
                    windowParam[9] = 0.0f;
                    break;
                case (int)WindowType.Chebyshev80://80 dB
                    windowSize = 7;
                    windowParam[0] = -165.6775938573387f;
                    windowParam[1] = 638.6206853579777f;
                    windowParam[2] = -693.5665448862951f;
                    windowParam[3] = 272.0580904584232f;
                    windowParam[4] = -35.714234112432310f;
                    windowParam[5] = 5.981350611100736f;
                    windowParam[6] = -0.099401698163953f;
                    windowParam[7] = 0.004876618884937f;
                    windowParam[8] = 0.0f;
                    windowParam[9] = 0.0f;
                    break;
                case (int)WindowType.VonHann:
                    windowSize = 7;
                    windowParam[0] = -35.6569885789057f;
                    windowParam[1] = 62.3995813533591f;
                    windowParam[2] = -6.11367282697140f;
                    windowParam[3] = -31.3575901301420f;
                    windowParam[4] = -0.115244487459543f;
                    windowParam[5] = 9.87581222686478f;
                    windowParam[6] = -0.000141282840344572f;
                    windowParam[7] = 7.79215137971980e-07f;
                    windowParam[8] = 0.0f;
                    windowParam[9] = 0.0f;
                    break;
                case (int)WindowType.Hamming:
                    windowSize = 7;
                    windowParam[0] = -32.5563808763587f;
                    windowParam[1] = 56.9735308008347f;
                    windowParam[2] = -5.58204910284797f;
                    windowParam[3] = -28.6308431623159f;
                    windowParam[4] = -0.105223227678585f;
                    windowParam[5] = 9.01704594626773f;
                    windowParam[6] = -0.000128997375965512f;
                    windowParam[7] = 0.0869572331964306f;
                    windowParam[8] = 0.0f;
                    windowParam[9] = 0.0f;
                    break;
                case (int)WindowType.Gauss://Sigma = 0.4
                    windowSize = 7;
                    windowParam[0] = 31.4014887172347f;
                    windowParam[1] = 106.320889497905f;
                    windowParam[2] = -170.809486986453f;
                    windowParam[3] = 51.8992982089132f;
                    windowParam[4] = 0.156587969922918f;
                    windowParam[5] = 3.43108559616593f;
                    windowParam[6] = 0.532508894382417f;
                    windowParam[7] = 0.0440576857015678f;
                    windowParam[8] = 0.0f;
                    windowParam[9] = 0.0f;
                    break;
                case (int)WindowType.BlackmannHarris:
                    windowSize = 7;
                    windowParam[0] = 960.594775448147f;
                    windowParam[1] = -1128.72343968861f;
                    windowParam[2] = 278.374950448407f;
                    windowParam[3] = 45.6484458963868f;
                    windowParam[4] = -5.96148680304882f;
                    windowParam[5] = 1.36456949748220f;
                    windowParam[6] = -0.0323511604381559f;
                    windowParam[7] = 0.000336081606954167f;
                    windowParam[8] = 0.0f;
                    windowParam[9] = 0.0f;
                    break;
                case (int)WindowType.SelfDefined:
                default:
                    windowSize = Convert.ToUInt16(num_Sys_Window_PolOrder.Value);
                    try
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            string objectName = "tB_Sys_Win_a" + (i).ToString();
                            TextBox tB = this.Controls.Find(objectName, true).FirstOrDefault() as TextBox;

                            windowParam[i] = Convert.ToSingle(tB.Text);
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Invalid window parameter!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    break;
            }
        }

        private void btn_Sys_initSystem_Click(object sender, EventArgs e)
        {
            bool groupDDS = SettingsCollector.BoardSelection == SettingsCollector.Board.RRC01 || SettingsCollector.BoardSelection == SettingsCollector.Board.TankGauging || SettingsCollector.BoardSelection == SettingsCollector.Board.BGT24 || SettingsCollector.BoardSelection == SettingsCollector.Board.RRC160_IntN || SettingsCollector.BoardSelection == SettingsCollector.Board.FindMine;
            bool groupFracN = SettingsCollector.BoardSelection == SettingsCollector.Board.FractionalN_PLL || SettingsCollector.BoardSelection == SettingsCollector.Board.RRC160_FracN;
            bool groupOffsetFrequ = SettingsCollector.BoardSelection == SettingsCollector.Board.RRC160_IntN || SettingsCollector.BoardSelection == SettingsCollector.Board.RRC160_FracN;

            updateWindowParameter(sender, e);

            //check if board is reachable and correct module is connected
            if (!com.openCOM(SettingsCollector.DSP_Com))
            {
                MessageBox.Show("Can't open " + SettingsCollector.DSP_Com + "!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!com.pingBoard())
            {
                MessageBox.Show("Can't ping board!\n Please check connection.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            char module;
            UInt16 version;
            if (!com.getBoardModuleAndVersion(out module, out version))
            {
                MessageBox.Show("Can't read board version!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!module.Equals(SettingsCollector.BoardDSPModule()))
            {
                DialogResult dialogResult = MessageBox.Show("Tried to initialize DSP module " + SettingsCollector.BoardDSPModule() + ", but module " + module + " is connected!\n\nDo you want to initialize anyway?", "Wrong DSP module connected!", MessageBoxButtons.YesNo);
                if (dialogResult != DialogResult.Yes)
                {
                    return;
                }
            }
            com.closeCOM();
            
            if (groupDDS)
            {
                //DDS
                btn_initAD5930_Click(sender, null);

                //PLL
                btn_initADF4108_Click(sender, null);
            }
            else if (groupFracN)
            {
                //HMC
                btn_initHMC_Click(sender, null);
            }
            else
            {
                throw new NotImplementedException();
            }

            if (SettingsCollector.BoardSelection == SettingsCollector.Board.BGT24)
            {
                //BGT
                btn_initBGT24_Click(sender, e);
            }

            //ADC
            UInt16 samples = Convert.ToUInt16(num_Sys_samples.Value);
            UInt16 samplesSkip = Convert.ToUInt16(num_Sys_Skip.Value);
            UInt16 nFFT = Convert.ToUInt16(num_Sys_FFT.Value);
            UInt16 stages = (UInt16)Math.Round((Math.Log10(nFFT) / Math.Log10(2)));
            UInt16 channel = Convert.ToUInt16(num_Sys_ADC_ch.Value);
            UInt16 gain = Convert.ToUInt16(num_Sys_gain.Value);
            UInt16 bias1 = Convert.ToUInt16(num_Sys_bias1.Value);
            UInt16 ramps = Convert.ToUInt16(num_Sys_ramps.Value);
            UInt16 delayRamps = Convert.ToUInt16(num_Sys_delay.Value);            

            if (!com.openCOM(SettingsCollector.DSP_Com))
            {
                MessageBox.Show("Can't open " + SettingsCollector.DSP_Com + "!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!com.ADC_Settings(samples, channel, gain))
            {
                MessageBox.Show("Couldn't initialize ADC!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (!com.setFFT_Params(samples, samplesSkip, nFFT, stages, ramps, delayRamps))
            {
                MessageBox.Show("Couldn't set FFT Parameter!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (!com.setResolution((float)(SettingsCollector.fft_settings.rres / 1000.0), (float)SettingsCollector.fft_settings.fres))
            {
                MessageBox.Show("Couldn't set FFT Parameter!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!com.setDACBias(1,bias1))
            {
                MessageBox.Show("Couldn't set DAC bias1!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (SettingsCollector.BoardSelection == SettingsCollector.Board.RRC01)
            {
                float lna_gain = Convert.ToSingle(num_Sys_RRC01_LNA_gain.Value);
                float tx_power = Convert.ToSingle(num_Sys_RRC01_TX_power.Value);
                float vref = Convert.ToSingle(num_Sys_RRC01_Vref_DAC.Value);
                float rx = Convert.ToSingle(num_Sys_RRC01_Vcc_RX.Value);
                float rx6 = Convert.ToSingle(num_Sys_RRC01_Vcc_RX6.Value);
                float tx3 = Convert.ToSingle(num_Sys_RRC01_Vcc_TX3.Value);
                float tx2 = Convert.ToSingle(num_Sys_RRC01_Vcc_TX2.Value);

                UInt16 supplyA = (UInt16)Math.Round(((rx-0.45)*255)/(vref*1.1));
                UInt16 supplyB = (UInt16)Math.Round(((rx6-0.45)*255)/(vref*1.1));
                UInt16 supplyC = (UInt16)Math.Round(((tx3-0.45)*255)/(vref*1.1));
                UInt16 supplyD = (UInt16)Math.Round(((tx2-0.45)*255)/(vref*1.1));
                UInt16 ctrlA = (UInt16)Math.Round(lna_gain*255);
                UInt16 ctrlB = (UInt16)Math.Round(tx_power*255);
                UInt16 ctrlC = supplyA;
                UInt16 ctrlD = supplyC;

                if(supplyB < supplyA)
                    ctrlC = supplyB;
                if (supplyD < supplyC)
                    ctrlD = supplyD;

                if (!com.evalE_DAC(supplyA, supplyB, supplyC, supplyD, ctrlA, ctrlB, ctrlC, ctrlD))
                {
                    MessageBox.Show("Couldn't set DAC bias!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else if (SettingsCollector.BoardSelection == SettingsCollector.Board.FindMine)
            {
                com.FindMine_EnableX2Line(cB_Sys_FindMineX2Multiplier.Checked);
            }

            if (!com.calcTwiddleFactors())
            {
                MessageBox.Show("Couldn't calculate twiddle factors!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!com.setWindowParams(windowSize, windowParam))
            {
                MessageBox.Show("Couldn't set window parameter!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!com.calcWindow())
            {
                MessageBox.Show("Couldn't calculate window!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            com.closeCOM();

            MessageBox.Show("System initialization successful", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void num_Sys_Window_PolOrder_ValueChanged(object sender, EventArgs e)
        {            
            UInt16 val = (UInt16)num_Sys_Window_PolOrder.Value;
            tB_Sys_Win_a0.Enabled = true;
            tB_Sys_Win_a1.Enabled = true;
            tB_Sys_Win_a2.Enabled = true;
            tB_Sys_Win_a3.Enabled = true;
            tB_Sys_Win_a4.Enabled = true;
            tB_Sys_Win_a5.Enabled = true;
            tB_Sys_Win_a6.Enabled = true;
            tB_Sys_Win_a7.Enabled = true;
            tB_Sys_Win_a8.Enabled = true;
            tB_Sys_Win_a9.Enabled = true;
            if (val < 9)
            {
                tB_Sys_Win_a9.Enabled = false;
                if (val < 8)
                {
                    tB_Sys_Win_a8.Enabled = false;
                    if (val < 7)
                    {
                        tB_Sys_Win_a7.Enabled = false;
                        if (val < 6)
                        {
                            tB_Sys_Win_a6.Enabled = false;
                            if (val < 5)
                            {
                                tB_Sys_Win_a5.Enabled = false;
                                if (val < 4)
                                {
                                    tB_Sys_Win_a4.Enabled = false;
                                    if (val < 3)
                                    {
                                        tB_Sys_Win_a3.Enabled = false;
                                        if (val < 2)
                                        {
                                            tB_Sys_Win_a2.Enabled = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void fullUpdateSystemTab()
        {
            EventArgs e = new EventArgs();
            updateSys_rampTime(this, e);
            updateSys_frequ_dev(this, e);
            cB_Sys_window_SelectedIndexChanged(this, e);
            cB_Sys_PLL_Prescale_SelectedIndexChanged(this, e);
            num_Sys_PLL_A_ValueChanged(this, e);
            num_Sys_PLL_B_ValueChanged(this, e);
            //The following will be called in the methods before
            //updateSys_PLL_ratio(this, e);
            //updateSysInfoBox(this, e);
        }

        private void updateSysInfoBox(object sender, EventArgs e)
        {
            bool groupDDS = SettingsCollector.BoardSelection == SettingsCollector.Board.RRC01 || SettingsCollector.BoardSelection == SettingsCollector.Board.TankGauging || SettingsCollector.BoardSelection == SettingsCollector.Board.BGT24 || SettingsCollector.BoardSelection == SettingsCollector.Board.RRC160_IntN || SettingsCollector.BoardSelection == SettingsCollector.Board.FindMine;
            bool groupFracN = SettingsCollector.BoardSelection == SettingsCollector.Board.FractionalN_PLL || SettingsCollector.BoardSelection == SettingsCollector.Board.RRC160_FracN;
            bool groupOffsetFrequ = SettingsCollector.BoardSelection == SettingsCollector.Board.RRC160_IntN || SettingsCollector.BoardSelection == SettingsCollector.Board.RRC160_FracN;
            
            //change names:
            if (groupDDS)
            {
                gB_Info2.Text = "DDS out";
            }
            else if (groupFracN)
            {
                gB_Info2.Text = "PLL RF in";
            }
            else
            {
                throw new NotImplementedException();
            }

            //double pllRatio = 0;
            double f_dev = 0;
            double ramp_time = 0;
            double fExtDDS = (double)num_Sys_extFreqInc.Value;
            double vco_pre = (double)num_Sys_VCOPrescaler.Value;
            double pll_a_ctr = (double)num_Sys_PLL_A.Value;
            double pll_b_ctr = (double)num_Sys_PLL_B.Value;
            double pll_prescale = (double)Math.Pow(2, 3 + cB_Sys_PLL_Prescale.SelectedIndex);
            double pll_r_ctr = 1;
            double samplingClock = (double)num_Sys_SampleClock.Value;
            double sysClockRampGeneration = (double)num_Sys_SysClock.Value;
            double f_mult = (double)num_Sys_freqMult.Value;
            if (SettingsCollector.BoardSelection == SettingsCollector.Board.FindMine)
            {
                if (cB_Sys_FindMineX2Multiplier.Checked)
                    f_mult = 2;
                else
                    f_mult = 1;
            }
            double f0 = (double)num_Sys_startFrequency.Value;
            double f_offset = (double)num_Sys_RRC160_OffsetFrequency.Value;
            double slope = (double)num_Sys_slope.Value;
            double samples = (double)num_Sys_samples.Value;
            double fftSize = (double)num_Sys_FFT.Value;
            try
            {
                f_dev = Convert.ToDouble(tB_Sys_fDev.Text);
                ramp_time = Convert.ToDouble(tB_Sys_rampTime.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid input value! Infobox won't be updated!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (groupDDS)
            {
                pll_r_ctr = 0;
                for (int i = 2; i <= 15; i++)
                {
                    string objectNameR = "cB_ADF4108_R_" + i.ToString("00");
                    CheckBox cB_R = this.Controls.Find(objectNameR, true).FirstOrDefault() as CheckBox;
                    if (cB_R.Checked)
                        pll_r_ctr += Math.Pow(2, i - 2); ;
                }
            }

            double HMCRefDiv = 1;
            double HMCRFDiv = 1;
            if (groupFracN)
            {
                HMCRefDiv = (double)num_HMC_RefDiv.Value;

                fExtDDS = fExtDDS / HMCRefDiv;
                sysClockRampGeneration = sysClockRampGeneration / HMCRefDiv;

                if (cB_HMC_AnEn_17.Checked)
                {
                    HMCRFDiv = 2;
                }
            }

            //----------calc-------------------------------------------------
            //1
            double f_ratio;
            double pll_mult = 1;
            if (groupDDS)
            {
                pll_mult = (pll_prescale * pll_b_ctr + pll_a_ctr)/pll_r_ctr;
                f_ratio = pll_mult * vco_pre * f_mult;
            }
            else if (groupFracN)
                f_ratio = vco_pre * f_mult * HMCRFDiv;
            else
                throw new NotImplementedException();

            //2 requested
            double f0_offset = f0;
            if (groupOffsetFrequ)
            {
                f0_offset -= f_offset;
            }

            double fstart2_requ = f0_offset / f_ratio;
            double slope2_requ = slope / f_ratio;
            double fstop2_requ = (f0_offset + f_dev) / f_ratio;
            double fdev2_requ = f_dev / f_ratio;

            //update 2
            double fstart2 = 0;
            double incr2 = 0;
            double slope2 = 0;
            double fstop2 = 0;
            //TG+RRC01
            UInt32 DDS_m = 0;
            UInt32 DDS_df = 0;
            //FracN
            UInt32 HMC_start_int = 0;
            UInt32 HMC_start_frac = 0;
            UInt32 HMC_step_frac = 0;
            UInt32 HMC_stop_int = 0;
            UInt32 HMC_stop_frac = 0;

            if (groupDDS)
            {
                DDS_m = (UInt32)Math.Round(fstart2_requ / sysClockRampGeneration * Math.Pow(2, 24));
                DDS_df = (UInt32)Math.Round(slope2_requ / fExtDDS / sysClockRampGeneration * Math.Pow(2, 24));
                fstart2 = DDS_m * sysClockRampGeneration / Math.Pow(2, 24);
                incr2 = DDS_df * sysClockRampGeneration / Math.Pow(2, 24);
                slope2 = fExtDDS * incr2;
                fstop2 = fstart2 + samples * (fExtDDS / samplingClock) * incr2;
            }
            else if (groupFracN)
            {
                UInt32 n_steps = (UInt32)Math.Ceiling(ramp_time * fExtDDS);
                HMC_start_int = (UInt32)Math.Floor(fstart2_requ / sysClockRampGeneration);
                HMC_start_frac = (UInt32)Math.Round((fstart2_requ - HMC_start_int * sysClockRampGeneration) / sysClockRampGeneration * Math.Pow(2, 24));
                HMC_step_frac = (UInt32)Math.Round(fdev2_requ / (n_steps * sysClockRampGeneration) * Math.Pow(2, 24));
                HMC_stop_int = (UInt32)(HMC_start_int + Math.Floor((HMC_start_frac + HMC_step_frac * n_steps) / Math.Pow(2, 24)));
                HMC_stop_frac = (UInt32)(Math.Floor((HMC_start_frac + HMC_step_frac * n_steps) % Math.Pow(2, 24)));

                fstart2 = (HMC_start_int + HMC_start_frac / Math.Pow(2, 24)) * sysClockRampGeneration;
                incr2 = (HMC_step_frac / Math.Pow(2, 24)) * sysClockRampGeneration;
                slope2 = ((HMC_step_frac * n_steps) / Math.Pow(2, 24)) * sysClockRampGeneration / ramp_time;
                fstop2 = (HMC_stop_int + HMC_stop_frac / Math.Pow(2, 24)) * sysClockRampGeneration;
            }
            else
            {
                throw new NotImplementedException();
            }
                

            double fdev2 = fstop2 - fstart2;
            
            //3 - VCO out
            double mult3;
            if (groupDDS)
                mult3 = pll_mult * vco_pre;
            else if (groupFracN)
                mult3 = vco_pre * HMCRFDiv;
            else
                throw new NotImplementedException();

            double fstart3 = fstart2 * mult3;
            double incr3 = incr2 * mult3 * 8;
            double fstop3 = fstop2 * mult3;
            double fdev3 = fstop3 - fstart3;
            double slope3 = slope2 * mult3;

            //4 - MWM out
            double fstop4 = fstop2 * f_ratio;
            if (groupOffsetFrequ)
            {
                fstop4 += f_offset;
            }

            //obtained values
            double startF_obtained = fstart2 * f_ratio;
            if (groupOffsetFrequ)
            {
                startF_obtained += f_offset;
            }
            double slope_obtained = slope2 * f_ratio;
            double fdev_obtained = slope_obtained * ramp_time;
            //ZF
            double rScale = 2 * slope_obtained / c0;
            //FFT
            SettingsCollector.fft_settings.fres = samplingClock / fftSize;
            SettingsCollector.fft_settings.rres = c0 / 2 / slope_obtained * SettingsCollector.fft_settings.fres * 1000;
            SettingsCollector.fft_settings.rmax = samplingClock / 2 * c0 / 2 / slope_obtained;

            //----------output-------------------------------------------------
            //1
            if (groupDDS)
            {
                tB_Sys_Info_f_ratio.Text = (fstop4 / fstop2).ToString("N3");
            }
            else if (groupFracN)
            {
                tB_Sys_Info_f_ratio.Text = (fstop4 / sysClockRampGeneration).ToString("N3");
            }
            else
            {
                throw new NotImplementedException();
            }

            //2
            tB_Sys_Info_2_startF_requ.Text = fstart2_requ.ToString("N0");
            tB_Sys_Info_2_deviation_requ.Text = fdev2_requ.ToString("N0");
            tB_Sys_Info_2_stopF_requ.Text = fstop2_requ.ToString("N0");
            tB_Sys_Info_2_slope_requ.Text = slope2_requ.ToString("N0");
            tB_Sys_Info_2_startF.Text = fstart2.ToString("N0");
            tB_Sys_Info_2_deviation.Text = fdev2.ToString("N0");
            tB_Sys_Info_2_increment.Text = incr2.ToString("N3");
            tB_Sys_Info_2_stopF.Text = fstop2.ToString("N0");
            tB_Sys_Info_2_slope.Text = slope2.ToString("N0");
            //3
            tB_Sys_Info_3_startF.Text = fstart3.ToString("N0");
            tB_Sys_Info_3_deviation.Text = fdev3.ToString("N0");
            tB_Sys_Info_3_increment.Text = incr3.ToString("N0");
            tB_Sys_Info_3_stopF.Text = fstop3.ToString("N0");
            tB_Sys_Info_3_slope.Text = slope3.ToString("N0");
            //4
            tB_Sys_Info_4_stop.Text = fstop4.ToString("N0");
            //ZF
            tB_Sys_Info_ZF_Rscale.Text = rScale.ToString("N0");
            //FFT
            tB_Sys_Info_FFT_fres.Text = SettingsCollector.fft_settings.fres.ToString("N3");
            tB_Sys_Info_FFT_rres.Text = SettingsCollector.fft_settings.rres.ToString("N4");
            tB_Sys_Info_FFT_maxR.Text = SettingsCollector.fft_settings.rmax.ToString("N1");
            //obtained values
            tB_Sys_startFrequency_obtained.Text = startF_obtained.ToString("N0");
            tB_Sys_slope_obtained.Text = slope_obtained.ToString("N0");
            tB_Sys_fDev_obtained.Text = fdev_obtained.ToString("N0");

            double fstop4_offsetfree = fstop4;
            if (groupOffsetFrequ)
            {
                fstop4_offsetfree -= f_offset;
            }
            if (groupDDS)
            {
                tB_Sys_fPD_Mult.Text = (fstop4_offsetfree / fstop2).ToString("N3");
            }
            else if (groupFracN)
            {
                tB_Sys_fPD_Mult.Text = (fstop4_offsetfree / sysClockRampGeneration).ToString("N3");
            }
            else
            {
                throw new NotImplementedException();
            }

            //update graphs
            clearCharts();

            //update registers
            if (groupDDS)
            {
                //DDS_m
                if (DDS_m < num_AD5930_startF.Minimum)
                    num_AD5930_startF.Value = num_AD5930_startF.Minimum;
                else if (DDS_m > num_AD5930_startF.Maximum)
                    num_AD5930_startF.Value = num_AD5930_startF.Maximum;
                else
                    num_AD5930_startF.Value = DDS_m;
                //DDS_df
                if (DDS_df < num_AD5930_deltaF.Minimum)
                    num_AD5930_deltaF.Value = num_AD5930_deltaF.Minimum;
                else if (DDS_df > num_AD5930_deltaF.Maximum)
                    num_AD5930_deltaF.Value = num_AD5930_deltaF.Maximum;
                else
                    num_AD5930_deltaF.Value = DDS_df;
            }
            else if (groupFracN)
            {
                //HMC_start_int
                if (HMC_start_int < num_HMC_FRegInt.Minimum)
                    num_HMC_FRegInt.Value = num_HMC_FRegInt.Minimum;
                else if (HMC_start_int > num_HMC_FRegInt.Maximum)
                    num_HMC_FRegInt.Value = num_HMC_FRegInt.Maximum;
                else
                    num_HMC_FRegInt.Value = HMC_start_int;
                //HMC_start_frac
                if (HMC_start_frac < num_HMC_FRegFrac.Minimum)
                    num_HMC_FRegFrac.Value = num_HMC_FRegFrac.Minimum;
                else if (HMC_start_frac > num_HMC_FRegFrac.Maximum)
                    num_HMC_FRegFrac.Value = num_HMC_FRegFrac.Maximum;
                else
                    num_HMC_FRegFrac.Value = HMC_start_frac;
                //HMC_step_frac
                if (HMC_step_frac < num_HMC_ModStep.Minimum)
                    num_HMC_ModStep.Value = num_HMC_ModStep.Minimum;
                else if (HMC_step_frac > num_HMC_ModStep.Maximum)
                    num_HMC_ModStep.Value = num_HMC_ModStep.Maximum;
                else
                    num_HMC_ModStep.Value = HMC_step_frac;
                //HMC_stop_int
                if (HMC_stop_int < num_HMC_ALTINT.Minimum)
                    num_HMC_ALTINT.Value = num_HMC_ALTINT.Minimum;
                else if (HMC_stop_int > num_HMC_ALTINT.Maximum)
                    num_HMC_ALTINT.Value = num_HMC_ALTINT.Maximum;
                else
                    num_HMC_ALTINT.Value = HMC_stop_int;
                //HMC_stop_frac
                if (HMC_stop_frac < num_HMC_ALTFRAC.Minimum)
                    num_HMC_ALTFRAC.Value = num_HMC_ALTFRAC.Minimum;
                else if (HMC_stop_frac > num_HMC_ALTFRAC.Maximum)
                    num_HMC_ALTFRAC.Value = num_HMC_ALTFRAC.Maximum;
                else
                    num_HMC_ALTFRAC.Value = HMC_stop_frac;

                //PLL N_counter in System Tab
                double n_ctr = HMC_start_int + HMC_start_frac / Math.Pow(2, 24);
                tB_Sys_HMC_PLL_N.Text = n_ctr.ToString("N7");
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void updateSys_rampTime(object sender, EventArgs e)
        {
            //update stages:
            tB_Sys_FFTStages.Text = (Math.Round((Math.Log10((double)num_Sys_FFT.Value) / Math.Log10(2)))).ToString();

            double sampleClock = (double)num_Sys_SampleClock.Value;
            tB_Sys_rampTime.Text = ((double)num_Sys_samples.Value / sampleClock).ToString();
            updateSysInfoBox(sender, e);
        }

        private void updateSys_frequ_dev(object sender, EventArgs e)
        {
            double rampTime = 0;
            try
            {
                rampTime = Convert.ToDouble(tB_Sys_rampTime.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid input value (ramp time)!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            tB_Sys_fDev.Text = ((double)num_Sys_slope.Value * rampTime).ToString("N0");
            updateSysInfoBox(sender, e);
        }

        private void cB_Sys_window_SelectedIndexChanged(object sender, EventArgs e)
        {
            gB_WindowParams.Visible = (cB_Sys_window.SelectedIndex == (int)WindowType.SelfDefined);
        }

        private void cB_Sys_PLL_Prescale_SelectedIndexChanged(object sender, EventArgs e)
        {
            //change prescale in AD5930
            BitArray b = new BitArray(new int[] { cB_Sys_PLL_Prescale.SelectedIndex });
            for (int i = 0; i < 2; i++)
            {
                string objectNameAB = "cB_ADF4108_F_" + (i + 22).ToString("00");
                CheckBox cB = this.Controls.Find(objectNameAB, true).FirstOrDefault() as CheckBox;

                if (cB != null)
                {
                    if (i < b.Length)
                    {
                        cB.Checked = b[i];
                    }
                    else
                    {
                        cB.Checked = false;
                    }
                }
            }

            updateSysInfoBox(sender, e);
        }

        private void num_Sys_PLL_A_ValueChanged(object sender, EventArgs e)
        {
            //change A_ctr in AD5930
            BitArray b = new BitArray(new int[] { (int)num_Sys_PLL_A.Value });
            for (int i = 0; i < 6; i++)
            {
                string objectNameAB = "cB_ADF4108_AB_" + (i + 2).ToString("00");
                CheckBox cB = this.Controls.Find(objectNameAB, true).FirstOrDefault() as CheckBox;

                if (cB != null)
                {
                    if (i < b.Length)
                    {
                        cB.Checked = b[i];
                    }
                    else
                    {
                        cB.Checked = false;
                    }
                }
            }
            updateSysInfoBox(sender, e);
        }

        private void num_Sys_PLL_B_ValueChanged(object sender, EventArgs e)
        {
            //change B_ctr in AD5930
            BitArray b = new BitArray(new int[] { (int)num_Sys_PLL_B.Value });
            for (int i = 0; i < 13; i++)
            {
                string objectNameAB = "cB_ADF4108_AB_" + (i + 8).ToString("00");
                CheckBox cB = this.Controls.Find(objectNameAB, true).FirstOrDefault() as CheckBox;

                if (cB != null)
                {
                    if (i < b.Length)
                    {
                        cB.Checked = b[i];
                    }
                    else
                    {
                        cB.Checked = false;
                    }
                }
            }

            updateSysInfoBox(sender, e);
        }
    }
}
