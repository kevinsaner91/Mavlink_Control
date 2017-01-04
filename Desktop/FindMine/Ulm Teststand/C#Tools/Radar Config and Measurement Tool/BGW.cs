using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.ComponentModel;
using System.IO;
using System.Globalization;
using System.Threading;

namespace Radar_Config_and_Measurement_Tool
{
    public partial class Main : Form
    {
        delegate void SaveImageCallback(string base_filename);
        private void Capture_SaveImage(string base_filename)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.chart_ZF.InvokeRequired)
            {
                SaveImageCallback d = new SaveImageCallback(Capture_SaveImage);
                this.Invoke(d, new object[] { base_filename });
            }
            else
            {
                string zf_filename = base_filename + "_raw.png";
                string fft_filename = base_filename + "_fft.png";
                this.chart_ZF.SaveImage(zf_filename, ChartImageFormat.Png);
                this.chart_Spectrum.SaveImage(fft_filename, ChartImageFormat.Png);
            }
        }

        delegate int GetIndexCallback(ComboBox cB);
        private int GetSelecetedIndex(ComboBox cB)
        {
            if (cB.InvokeRequired)
            {
                GetIndexCallback d = new GetIndexCallback(GetSelecetedIndex);
                return (int)(this.Invoke(d, new object[] { cB }));
            }
            else
                return cB.SelectedIndex;
        }  


        delegate void SetTextCallback(string temp, string gain, uint ctr);
        private void Capture_UpdateTexts(string temp, string gain, uint ctr)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.label_temperature.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(Capture_UpdateTexts);
                this.Invoke(d, new object[] { temp, gain, ctr });
            }
            else
            {
                this.label_temperature.Text = temp;
                this.label_gain.Text = gain;
                this.tB_CycleCounter.Text = ctr.ToString();
            }
        }

        delegate void UpdateChartsCallback(UInt16[] zfData, float[] MagData);
        private void Capture_UpdateCharts(UInt16[] zfData, float[] MagData)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.label_temperature.InvokeRequired)
            {
                UpdateChartsCallback d = new UpdateChartsCallback(Capture_UpdateCharts);
                this.Invoke(d, new object[] { zfData, MagData });
            }
            else
            {
                //update chart ZF
                chart_ZF.ChartAreas["area"].AxisX.Minimum = 0;
                chart_ZF.ChartAreas["area"].AxisX.Maximum = zfData.Length;
                //chart_ZF.ChartAreas["area"].AxisX.Interval = Math.Floor(zfData.Length / 8.0);
                chart_ZF.ChartAreas["area"].AxisY.Minimum = 0;
                chart_ZF.ChartAreas["area"].AxisY.Maximum = 4096;
                chart_ZF.ChartAreas["area"].AxisY.Interval = 512;


                chart_ZF.Series["data"].Points.Clear();
                UInt32 ctr = 0;
                foreach (UInt16 val in zfData)
                {
                    UInt16 rpc = (UInt16)num_Sys_ramps.Value;
                    chart_ZF.Series["data"].Points.AddXY(ctr, val / rpc);
                    ctr++;
                }

                //update chart Mag
                chart_Spectrum.ChartAreas["area"].AxisX.Minimum = 0;
                chart_Spectrum.ChartAreas["area"].AxisX.Maximum = (int)(SettingsCollector.fft_settings.rmax+1);
                chart_Spectrum.ChartAreas["area"].AxisY.Minimum = 0;

                chart_Spectrum.Series["data"].Points.Clear();
                double distance = 0;
                DataPoint maxMagnitude = new DataPoint(0,0);
                foreach (float val in MagData)
                {
                    DataPoint nextPoint = new DataPoint(distance, val);
                    nextPoint.ToolTip = "Distance:\t" + distance.ToString("#.#####") + " m\nAmplitude:\t" + val.ToString("#.#####") + " dB";
                    chart_Spectrum.Series["data"].Points.Add(nextPoint);
                    if ((val+1) > chart_Spectrum.ChartAreas["area"].AxisY.Maximum)
                    {
                        chart_Spectrum.ChartAreas["area"].AxisY.Maximum = Math.Ceiling((val+1)/10)*10;
                    }

                    if (val > maxMagnitude.YValues[0])
                    {
                        maxMagnitude = nextPoint;
                    }

                    distance += (SettingsCollector.fft_settings.rres / 1000.0);
                }

                //highlight maximum
                maxMagnitude.MarkerStyle = MarkerStyle.Cross;
                maxMagnitude.MarkerSize = 8;
                maxMagnitude.MarkerColor = System.Drawing.Color.Red;
                lbl_Capture_MaxInfo.Text = "Maximum: " + maxMagnitude.YValues[0].ToString("0.0") + " dB @ " + maxMagnitude.XValue.ToString("0.000") + " m";

                //update intervals
                chart_Spectrum.ChartAreas["area"].AxisX.Interval = (chart_Spectrum.ChartAreas["area"].AxisX.ScaleView.ViewMaximum - chart_Spectrum.ChartAreas["area"].AxisX.ScaleView.ViewMinimum) / 8.0;
                chart_Spectrum.ChartAreas["area"].AxisY.Interval = (chart_Spectrum.ChartAreas["area"].AxisY.ScaleView.ViewMaximum - chart_Spectrum.ChartAreas["area"].AxisY.ScaleView.ViewMinimum) / 8.0;
                chart_ZF.ChartAreas["area"].AxisX.Interval = (chart_ZF.ChartAreas["area"].AxisX.ScaleView.ViewMaximum - chart_ZF.ChartAreas["area"].AxisX.ScaleView.ViewMinimum) / 8.0;
                chart_ZF.ChartAreas["area"].AxisY.Interval = (chart_ZF.ChartAreas["area"].AxisY.ScaleView.ViewMaximum - chart_ZF.ChartAreas["area"].AxisY.ScaleView.ViewMinimum) / 8.0;
            }
        }

        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            bool groupDDS = SettingsCollector.BoardSelection == SettingsCollector.Board.RRC01 || SettingsCollector.BoardSelection == SettingsCollector.Board.TankGauging || SettingsCollector.BoardSelection == SettingsCollector.Board.BGT24 || SettingsCollector.BoardSelection == SettingsCollector.Board.RRC160_IntN || SettingsCollector.BoardSelection == SettingsCollector.Board.FindMine;
            bool groupFracN = SettingsCollector.BoardSelection == SettingsCollector.Board.FractionalN_PLL || SettingsCollector.BoardSelection == SettingsCollector.Board.RRC160_FracN;
            bool groupOffsetFrequ = SettingsCollector.BoardSelection == SettingsCollector.Board.RRC160_IntN || SettingsCollector.BoardSelection == SettingsCollector.Board.RRC160_FracN;

            bool noErrors = true;
            if (!com.openCOM(SettingsCollector.DSP_Com))
            {
                e.Cancel = true;
                MessageBox.Show("Can't open " + SettingsCollector.DSP_Com + "!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                noErrors = false;
            }

            if (noErrors)
            {
                if (!com.AD5305_DAC(120, 155, 112)) //TODO: Get input data
                {
                    e.Cancel = true;
                    MessageBox.Show("Communication errors occured during performing ramp.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    noErrors = false;
                }
            }

            if (noErrors)
            {
                while (BGW_Task.description == TASKDesc.InfiniteCapture_noCSV || BGW_Task.description == TASKDesc.Capture2CSV_ExternalStop ||BGW_Task.captureCtr < BGW_Task.amount)
                {
                    if ((worker.CancellationPending == true))
                    {
                        e.Cancel = true;
                        break;
                    }
                    else
                    {
                        float temperature = 0;
                        UInt16 gain = 0;
                        UInt16[] zf_data = { };
                        float[] mag_data = { };
                        UInt32 zf_length = 0, mag_length = 0;

                        int retries = 10;
                        bool done = false;

                        while (!done)
                        {
                            if (!com.performRamp((UInt16)num_HKnum.Value, out zf_data, out zf_length, out mag_data, out mag_length, out temperature, out gain))
                            {
                                retries--;
                                if (retries < 1)
                                {
                                    e.Cancel = true;
                                    MessageBox.Show("Communication errors occured during performing ramp.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    break;
                                }

                                if ((worker.CancellationPending == true))
                                {
                                    e.Cancel = true;
                                    break;
                                }
                            }
                            else
                            {
                                done = true;
                            }
                        }

                        //update Charts
                        Capture_UpdateCharts(zf_data, mag_data);

                        //show Temperature and Gain and update ctr
                        BGW_Task.captureCtr++;
                        Capture_UpdateTexts("Temperature: " + temperature.ToString("00.00") + " °C", "Actual Gain: " + gain.ToString(), BGW_Task.captureCtr);

                        //create files
                        if (BGW_Task.description == TASKDesc.Capture2CSV_Amount || BGW_Task.description == TASKDesc.Capture2CSV_ExternalStop || BGW_Task.description == TASKDesc.Automation)
                        {
                            if (BGW_Task.captureCtr == 1)
                            {
                                //first eval -> create data.m
                                string m_filename = BGW_Task.folder + "\\data.m";
                                using (var stream = File.CreateText(m_filename))
                                {
                                    stream.WriteLine("module = '" + SettingsCollector.BoardName() + "';");
                                    if (BGW_Task.description == TASKDesc.Automation)
                                        stream.WriteLine("rad.dist = " + BGW_Task.measuredDistance.ToString() + ";");
                                    stream.WriteLine("rad.f0 = " + Convert.ToDouble(tB_Sys_startFrequency_obtained.Text).ToString() + ";");
                                    stream.WriteLine("rad.S = " + Convert.ToDouble(tB_Sys_slope_obtained.Text).ToString() + ";");
                                    stream.WriteLine("rad.df = " + Convert.ToDouble(tB_Sys_fDev_obtained.Text).ToString() + ";");
                                    stream.WriteLine("rad.T = " + Convert.ToDouble(tB_Sys_rampTime.Text).ToString() + ";");
                                    stream.WriteLine("arch.nt = " + Convert.ToDouble(tB_Sys_fPD_Mult.Text).ToString() + ";");
                                    stream.WriteLine("arch.vv = " + num_Sys_freqMult.Value.ToString() + ";");
                                    stream.WriteLine("arch.sysclk = " + num_Sys_SysClock.Value.ToString() + ";");
                                    stream.WriteLine("rad.fs = " + num_Sys_SampleClock.Value.ToString() + ";");
                                    stream.WriteLine("rad.DDS_fext = " + num_Sys_extFreqInc.Value.ToString() + ";");

                                    if (groupOffsetFrequ)
                                    {
                                        stream.WriteLine("arch.foffset = " + num_Sys_RRC160_OffsetFrequency.Value.ToString() + ";");
                                    }
                                    else
                                    {
                                        stream.WriteLine("arch.foffset = 0;");
                                    }

                                    stream.WriteLine("arch.vco_pre = " + num_Sys_VCOPrescaler.Value.ToString() + ";");
                                    if (groupDDS)
                                    {
                                        stream.WriteLine("arch.pll_p = " + GetSelecetedIndex(cB_Sys_PLL_Prescale).ToString() + ";");
                                        stream.WriteLine("arch.pll_a = " + num_Sys_PLL_A.Value.ToString() + ";");
                                        stream.WriteLine("arch.pll_b = " + num_Sys_PLL_B.Value.ToString() + ";");
                                    }
                                    else if (groupFracN)
                                    {
                                        stream.WriteLine("arch.pll_p = 0;");
                                        stream.WriteLine("arch.pll_n = " + Convert.ToDouble(tB_Sys_HMC_PLL_N.Text).ToString() + ";");
                                    }
                                    else
                                    {
                                        throw new NotImplementedException();
                                    }

                                    stream.WriteLine("rad.npr = " + num_Sys_samples.Value.ToString() + ";");
                                    stream.WriteLine("ev.nskip = " + num_Sys_Skip.Value.ToString() + ";");
                                    stream.WriteLine("ev.nfft = " + num_Sys_FFT.Value.ToString() + ";");
                                    stream.WriteLine("ev.fft_stages = " + Convert.ToDouble(tB_Sys_FFTStages.Text).ToString() + ";");
                                    stream.WriteLine("ev.wtype = " + GetSelecetedIndex(cB_Sys_window).ToString() + ";");
                                    stream.WriteLine("arch.ch = " + num_Sys_ADC_ch.Value.ToString() + ";");
                                    stream.WriteLine("arch.gsetting = " + num_Sys_gain.Value.ToString() + ";");
                                    stream.WriteLine("arch.bias1 = " + num_Sys_bias1.Value.ToString() + ";");
                                    stream.WriteLine("rad.rpc = " + num_Sys_ramps.Value.ToString() + ";");
                                    stream.WriteLine("rad.rdelay = " + num_Sys_delay.Value.ToString() + ";");

                                    stream.WriteLine("win.nwin = " + windowSize.ToString() + ";");
                                    try
                                    {
                                        stream.WriteLine("win.a0 = " + windowParam[0].ToString() + ";");
                                        stream.WriteLine("win.a1 = " + windowParam[1].ToString() + ";");
                                        stream.WriteLine("win.a2 = " + windowParam[2].ToString() + ";");
                                        stream.WriteLine("win.a3 = " + windowParam[3].ToString() + ";");
                                        stream.WriteLine("win.a4 = " + windowParam[4].ToString() + ";");
                                        stream.WriteLine("win.a5 = " + windowParam[5].ToString() + ";");
                                        stream.WriteLine("win.a6 = " + windowParam[6].ToString() + ";");
                                        stream.WriteLine("win.a7 = " + windowParam[7].ToString() + ";");
                                        stream.WriteLine("win.a8 = " + windowParam[8].ToString() + ";");
                                        stream.WriteLine("win.a9 = " + windowParam[9].ToString() + ";");
                                    }
                                    catch (Exception)
                                    {
                                        MessageBox.Show("Please check window parameter!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                            string base_filename = BGW_Task.folder + "\\data" + BGW_Task.captureCtr.ToString("000000");
                            //create csv
                            string csv_filename = base_filename + ".csv";
                            using (var stream = File.CreateText(csv_filename))
                            {
                                if (BGW_Task.description == TASKDesc.Automation)
                                    stream.WriteLine("Data: " + zf_length + " points; MeasuredDistance: " + BGW_Task.measuredDistance.ToString() + "; Creation: " + DateTime.Now.ToString("dd.MM.yy H:mm:ss"));
                                else
                                    stream.WriteLine("Data: " + zf_length + " points; Creation: " + DateTime.Now.ToString("dd.MM.yy H:mm:ss"));
                                foreach (UInt16 data in zf_data)
                                {
                                    stream.WriteLine(data);
                                }
                            }

                            //save images
                            if(BGW_Task.savePictures)
                                Capture_SaveImage(base_filename);
                        }

                        if (BGW_Task.description == TASKDesc.InfiniteCapture_noCSV || BGW_Task.description == TASKDesc.Capture2CSV_Amount || BGW_Task.description == TASKDesc.Capture2CSV_ExternalStop)
                        {
                            for (int i = 1; i < num_Capture_CycleDelay.Value; i++) //Normal cycle is about 1 second -> start at i = 1!
                            {//Sleep 1 second and check if cancelation Pending in between
                                Thread.Sleep(500);
                                if (worker.CancellationPending == true)
                                {
                                    e.Cancel = true;
                                    break;
                                }

                                Thread.Sleep(500);
                                if (worker.CancellationPending == true)
                                {
                                    e.Cancel = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            com.closeCOM();
            if (BGW_Task.description != TASKDesc.PreCapture && BGW_Task.description != TASKDesc.Automation)
            {
                btn_Capture_StartStop.Text = "start capture";
                btn_startAutomation.Text = "start automation";
                btn_Capture_StartStop.Enabled = true;
                cB_cap2CSV.Enabled = true;
                btn_startAutomation.Enabled = true;
            }


            if (BGW_Task.closing_pending)
            {
                this.Close();
            }

            if (e.Cancelled)
            {
                automationCancelationPending = true;
            }
        }
    }
}
