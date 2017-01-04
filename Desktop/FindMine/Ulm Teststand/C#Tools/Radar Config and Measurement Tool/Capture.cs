using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Radar_Config_and_Measurement_Tool
{
    public partial class Main : Form
    {
        private void clearCharts()
        {
            //init chart ZF
            chart_ZF.ChartAreas.Clear();
            chart_ZF.ChartAreas.Add("area");
            chart_ZF.ChartAreas["area"].AxisX.Minimum = 0;
            chart_ZF.ChartAreas["area"].AxisX.Maximum = 4095;
            chart_ZF.ChartAreas["area"].AxisX.Interval = 512;
            chart_ZF.ChartAreas["area"].AxisY.Minimum = 0;
            chart_ZF.ChartAreas["area"].AxisY.Maximum = 4096;
            chart_ZF.ChartAreas["area"].AxisY.Interval = 512;
            chart_ZF.ChartAreas["area"].AxisY.Title = "ADC value";

            chart_ZF.ChartAreas["area"].CursorX.IsUserSelectionEnabled = true;
            chart_ZF.ChartAreas["area"].CursorY.IsUserSelectionEnabled = true;
            chart_ZF.ChartAreas["area"].AxisX.ScaleView.Zoomable = true;
            chart_ZF.ChartAreas["area"].AxisY.ScaleView.Zoomable = true;
            chart_ZF.ChartAreas["area"].CursorX.AutoScroll = true;
            chart_ZF.ChartAreas["area"].CursorY.AutoScroll = true;

            chart_ZF.Series.Clear();
            chart_ZF.Series.Add("data");
            chart_ZF.Series["data"].ChartType = SeriesChartType.Line;
            chart_ZF.Series["data"].Points.AddXY(0, 2048);

            //init chart Mag
            chart_Spectrum.ChartAreas.Clear();
            chart_Spectrum.ChartAreas.Add("area");
            chart_Spectrum.ChartAreas["area"].AxisX.Minimum = 0;
            chart_Spectrum.ChartAreas["area"].AxisX.Maximum = SettingsCollector.fft_settings.rmax;
            chart_Spectrum.ChartAreas["area"].AxisX.Interval = Math.Floor(SettingsCollector.fft_settings.rmax / 8.0);
            chart_Spectrum.ChartAreas["area"].AxisX.Title = "Distance [m]";
            chart_Spectrum.ChartAreas["area"].AxisY.Minimum = 0;
            btn_CaptureResetAmplitudeAxis_Click(this, null);
            chart_Spectrum.ChartAreas["area"].AxisY.Title = "Amplitude [dB]";

            chart_Spectrum.ChartAreas["area"].CursorX.IsUserSelectionEnabled = true;
            chart_Spectrum.ChartAreas["area"].CursorY.IsUserSelectionEnabled = true;
            chart_Spectrum.ChartAreas["area"].AxisX.ScaleView.Zoomable = true;
            chart_Spectrum.ChartAreas["area"].AxisY.ScaleView.Zoomable = true;
            chart_Spectrum.ChartAreas["area"].CursorX.AutoScroll = true;
            chart_Spectrum.ChartAreas["area"].CursorY.AutoScroll = true;

            chart_Spectrum.Series.Clear();
            chart_Spectrum.Series.Add("data");
            chart_Spectrum.Series["data"].ChartType = SeriesChartType.Line;
            chart_Spectrum.Series["data"].Points.AddXY(0, 100);
        }

        private void cB_cap2CSV_CheckedChanged(object sender, EventArgs e)
        {
            gB_Capture_CSV_Settings.Enabled = cB_cap2CSV.Checked;
        }

        private void btn_startStopMultiple_Click(object sender, EventArgs e)
        {
            if (btn_Capture_StartStop.Text.StartsWith("start"))
            {
                //start Background worker
                if (bgw.IsBusy != true)
                {
                    BGW_Task.captureCtr = 0;
                    BGW_Task.savePictures = captureRAWAndAndFFTDataTopngToolStripMenuItem.Checked;

                    if (cB_cap2CSV.Checked)
                    {//Prepare Capture to csv
                        //get folder
                        FolderBrowserDialog browser = new FolderBrowserDialog();
                        browser.Description = "Select folder where the CSV will be written to...";
                        browser.ShowNewFolderButton = true;
                        if (browser.ShowDialog() == DialogResult.OK)
                        {
                            BGW_Task.folder = browser.SelectedPath;
                        }
                        else
                        {
                            MessageBox.Show("Capture stopped", "No folder selected.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        if (rB_CaptureCSV_amount.Checked)
                        {
                            BGW_Task.description = TASKDesc.Capture2CSV_Amount;
                            BGW_Task.amount = Convert.ToUInt16(num_Cap2CSV_amount.Value);
                            bgw.RunWorkerAsync();
                        }
                        else if (rB_CaptureCSV_manual.Checked)
                        {
                            BGW_Task.description = TASKDesc.Capture2CSV_ExternalStop;
                            BGW_Task.amount = 1;
                            bgw.RunWorkerAsync();
                        }
                        else if (rB_CaptureCSV_time.Checked)
                        {
                            BGW_Task.description = TASKDesc.Capture2CSV_ExternalStop;
                            BGW_Task.amount = 1;
                            bgw.RunWorkerAsync();

                            //Setup timer
                            captureCSVtimer.Interval = (int)(num_Cap2CSV_seconds.Value * 1000);
                            captureCSVtimer.Enabled = true;
                        }
                        else
                        {
                            //No radio button selected -> should never occur!
                            MessageBox.Show("Please check csv settings. No cancel criteria selected.", "Failure", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                    else
                    { //no csv capture
                        BGW_Task.description = TASKDesc.InfiniteCapture_noCSV;
                        BGW_Task.amount = 1;
                        bgw.RunWorkerAsync();
                    }

                    btn_Capture_StartStop.Text = "stop capture";
                    cB_cap2CSV.Enabled = false;
                    btn_startAutomation.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Backgroundworker busy. Please stop active capture before starting a new one.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                //stop Backgroundworker. btn_text will be changed in WorkerComplete
                btn_Capture_StartStop.Text = "pending...";
                btn_Capture_StartStop.Enabled = false;
                bgw.CancelAsync();
            }
        }

        private void captureCSVtimer_Tick(object sender, EventArgs e)
        {
            captureCSVtimer.Enabled = false;
            bgw.CancelAsync();
        }

        private void chart_Spectrum_AxisViewChanged(object sender, ViewEventArgs e)
        {
            chart_Spectrum.ChartAreas["area"].AxisX.Interval = (chart_Spectrum.ChartAreas["area"].AxisX.ScaleView.ViewMaximum - chart_Spectrum.ChartAreas["area"].AxisX.ScaleView.ViewMinimum) / 8.0;
            chart_Spectrum.ChartAreas["area"].AxisY.Interval = (chart_Spectrum.ChartAreas["area"].AxisY.ScaleView.ViewMaximum - chart_Spectrum.ChartAreas["area"].AxisY.ScaleView.ViewMinimum) / 8.0;
        }

        private void chart_ZF_AxisViewChanged(object sender, ViewEventArgs e)
        {
            chart_ZF.ChartAreas["area"].AxisX.Interval = (chart_ZF.ChartAreas["area"].AxisX.ScaleView.ViewMaximum - chart_ZF.ChartAreas["area"].AxisX.ScaleView.ViewMinimum) / 8.0;
            chart_ZF.ChartAreas["area"].AxisY.Interval = (chart_ZF.ChartAreas["area"].AxisY.ScaleView.ViewMaximum - chart_ZF.ChartAreas["area"].AxisY.ScaleView.ViewMinimum) / 8.0;
        }

        private void btn_CaptureResetAmplitudeAxis_Click(object sender, EventArgs e)
        {
            chart_Spectrum.ChartAreas["area"].AxisY.Maximum = 40;
            chart_Spectrum.ChartAreas["area"].AxisY.Interval = 10;
        }
    }
}
