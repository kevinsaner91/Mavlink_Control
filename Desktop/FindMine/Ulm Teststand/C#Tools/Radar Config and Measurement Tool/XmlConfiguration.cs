using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace Radar_Config_and_Measurement_Tool
{
    public partial class Main : Form
    {
        private bool loadConfigurationFile(string configFilePath, object sender, EventArgs e, bool allowBoardChange = false)
        {
            XmlReader reader = XmlReader.Create(configFilePath);
            bool startFound = false;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "RadarConfigMeasurementTool")
                {
                    startFound = true;
                    SettingsCollector.Board configBoard = (SettingsCollector.Board)Convert.ToUInt16(reader["Module"]);
                    if (allowBoardChange)
                    {
                        SettingsCollector.BoardSelection = configBoard;
                        updateTabSelection(sender, e);
                    }
                    else
                    {
                        if (SettingsCollector.BoardSelection != configBoard)
                        {
                            MessageBox.Show("The given Configuration file is not valid for the actual selected Board type", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            reader.Close();
                            return false;
                        }
                    }

                    bool groupDDS = SettingsCollector.BoardSelection == SettingsCollector.Board.RRC01 || SettingsCollector.BoardSelection == SettingsCollector.Board.TankGauging || SettingsCollector.BoardSelection == SettingsCollector.Board.BGT24 || SettingsCollector.BoardSelection == SettingsCollector.Board.RRC160_IntN || SettingsCollector.BoardSelection == SettingsCollector.Board.FindMine;
                    bool groupFracN = SettingsCollector.BoardSelection == SettingsCollector.Board.FractionalN_PLL || SettingsCollector.BoardSelection == SettingsCollector.Board.RRC160_FracN;
                    bool groupOffsetFrequ = SettingsCollector.BoardSelection == SettingsCollector.Board.RRC160_IntN || SettingsCollector.BoardSelection == SettingsCollector.Board.RRC160_FracN;

                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        reader.Read();

                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "System")
                        {
                            num_Sys_startFrequency.Value = Convert.ToDecimal(reader["startF"]);
                            num_Sys_slope.Value = Convert.ToDecimal(reader["slope"]);
                            num_Sys_freqMult.Value = Convert.ToDecimal(reader["f_mult"]);
                            cB_Sys_FindMineX2Multiplier.Checked = (num_Sys_freqMult.Value > 1);
                            num_Sys_SysClock.Value = Convert.ToDecimal(reader["sys_clk"]);
                            num_Sys_SampleClock.Value = Convert.ToDecimal(reader["sample_clk"]);
                            num_Sys_extFreqInc.Value = Convert.ToDecimal(reader["f_ext"]);
                            if (groupOffsetFrequ)
                            {
                                num_Sys_RRC160_OffsetFrequency.Value = Convert.ToDecimal(reader["f_offset"]);
                            }
                            num_Sys_VCOPrescaler.Value = Convert.ToDecimal(reader["vco_pre"]);
                            if (groupDDS)
                            {
                                cB_Sys_PLL_Prescale.SelectedIndex = Convert.ToInt16(reader["pll_pre"]);
                                num_Sys_PLL_A.Value = Convert.ToDecimal(reader["pll_a_ctr"]);
                                num_Sys_PLL_B.Value = Convert.ToDecimal(reader["pll_b_ctr"]);
                            }
                            num_Sys_samples.Value = Convert.ToDecimal(reader["samples"]);
                            num_Sys_Skip.Value = Convert.ToDecimal(reader["skip"]);
                            num_Sys_FFT.Value = Convert.ToDecimal(reader["FFT_size"]);
                            num_Sys_ADC_ch.Value = Convert.ToDecimal(reader["adc_ch"]);
                            num_Sys_gain.Value = Convert.ToDecimal(reader["gain"]);
                            if (SettingsCollector.BoardSelection != SettingsCollector.Board.RRC160_IntN && SettingsCollector.BoardSelection != SettingsCollector.Board.RRC160_FracN) 
                                num_Sys_bias1.Value = Convert.ToDecimal(reader["dac_bias1"]);
                            num_Sys_ramps.Value = Convert.ToDecimal(reader["ramps_p_c"]);
                            num_Sys_delay.Value = Convert.ToDecimal(reader["delay"]);
                        }

                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Window")
                        {
                            cB_Sys_window.SelectedIndex = Convert.ToInt16(reader["type"]);
                            if (cB_Sys_window.SelectedIndex == 6)
                            {
                                num_Sys_Window_PolOrder.Value = Convert.ToDecimal(reader["nwin"]);
                                tB_Sys_Win_a0.Text = reader["a0"];
                                tB_Sys_Win_a1.Text = reader["a1"];
                                tB_Sys_Win_a2.Text = reader["a2"];
                                tB_Sys_Win_a3.Text = reader["a3"];
                                tB_Sys_Win_a4.Text = reader["a4"];
                                tB_Sys_Win_a5.Text = reader["a5"];
                                tB_Sys_Win_a6.Text = reader["a6"];
                                tB_Sys_Win_a7.Text = reader["a7"];
                                tB_Sys_Win_a8.Text = reader["a8"];
                                tB_Sys_Win_a9.Text = reader["a9"];
                            }
                        }

                        if (SettingsCollector.BoardSelection == SettingsCollector.Board.RRC01)
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "supplyDAC")
                            {
                                num_Sys_RRC01_LNA_gain.Value = Convert.ToDecimal(reader["LNA_gain"]);
                                num_Sys_RRC01_TX_power.Value = Convert.ToDecimal(reader["TX_power"]);
                                num_Sys_RRC01_Vref_DAC.Value = Convert.ToDecimal(reader["Vref_DAC"]);
                                num_Sys_RRC01_Vcc_RX.Value = Convert.ToDecimal(reader["Vcc_RX"]);
                                num_Sys_RRC01_Vcc_RX6.Value = Convert.ToDecimal(reader["Vcc_RX6"]);
                                num_Sys_RRC01_Vcc_TX3.Value = Convert.ToDecimal(reader["Vcc_TX3"]);
                                num_Sys_RRC01_Vcc_TX2.Value = Convert.ToDecimal(reader["Vcc_TX2"]);
                            }
                        }

                        if (groupDDS)
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "PLL")
                            {
                                UInt32 PLL_ref = Convert.ToUInt32(reader["Ref"],16);
                                UInt32 PLL_ab = Convert.ToUInt32(reader["AB"], 16);
                                UInt32 PLL_func = Convert.ToUInt32(reader["Function"], 16);

                                ADF4108_setFromConfig(PLL_ref, PLL_ab, PLL_func);
                            }

                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "DDS")
                            {
                                UInt16 DDS_Control = Convert.ToUInt16(reader["Control"], 16);
                                UInt16 DDS_fstart_lsb = Convert.ToUInt16(reader["FSTART_lsb"], 16);
                                UInt16 DDS_fstart_msb = Convert.ToUInt16(reader["FSTART_msb"], 16);
                                UInt16 DDS_deltaF_lsb = Convert.ToUInt16(reader["DeltaF_lsb"], 16);
                                UInt16 DDS_deltaF_msb = Convert.ToUInt16(reader["DeltaF_msb"], 16);
                                UInt16 DDS_Ninc = Convert.ToUInt16(reader["Ninc"], 16);
                                UInt16 DDS_tINT = Convert.ToUInt16(reader["tINT"], 16);
                                UInt16 DDS_TBURST = Convert.ToUInt16(reader["TBURST"], 16);

                                AD5930_setFromConfig(DDS_Control, DDS_fstart_lsb, DDS_fstart_msb, DDS_deltaF_lsb, DDS_deltaF_msb, DDS_Ninc, DDS_tINT, DDS_TBURST);
                            }
                        }

                        if (groupFracN)
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "HMC")
                            {
                                UInt32 HMC_01 = Convert.ToUInt32(reader["r01"], 16);
                                UInt32 HMC_02 = Convert.ToUInt32(reader["r02"], 16);
                                UInt32 HMC_03 = Convert.ToUInt32(reader["r03"], 16);
                                UInt32 HMC_04 = Convert.ToUInt32(reader["r04"], 16);
                                UInt32 HMC_05 = Convert.ToUInt32(reader["r05"], 16);
                                UInt32 HMC_06 = Convert.ToUInt32(reader["r06"], 16);
                                UInt32 HMC_07 = Convert.ToUInt32(reader["r07"], 16);
                                UInt32 HMC_08 = Convert.ToUInt32(reader["r08"], 16);
                                UInt32 HMC_09 = Convert.ToUInt32(reader["r09"], 16);
                                UInt32 HMC_10 = Convert.ToUInt32(reader["r10"], 16);
                                UInt32 HMC_11 = Convert.ToUInt32(reader["r11"], 16);
                                UInt32 HMC_12 = Convert.ToUInt32(reader["r12"], 16);
                                UInt32 HMC_13 = Convert.ToUInt32(reader["r13"], 16);
                                UInt32 HMC_15 = Convert.ToUInt32(reader["r15"], 16);

                                HMC703_setFromConfig(HMC_01, HMC_02, HMC_03, HMC_04, HMC_05, HMC_06, HMC_07, HMC_08, HMC_09, HMC_10, HMC_11, HMC_12, HMC_13, HMC_15);
                            }
                        }
                        else if (SettingsCollector.BoardSelection == SettingsCollector.Board.BGT24)
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "BGT24")
                            {
                                UInt32 BGT_control = Convert.ToUInt32(reader["control"], 16);
                                BGT24_setFromConfig(BGT_control);
                            }
                        }
                    }
                }
            }
            reader.Close();
            fullUpdateSystemTab();
            return startFound;
        }

        private bool saveConfigurationFile(string configFilePath, object sender, EventArgs e, bool hidden = false)
        {
            bool groupDDS = SettingsCollector.BoardSelection == SettingsCollector.Board.RRC01 || SettingsCollector.BoardSelection == SettingsCollector.Board.TankGauging || SettingsCollector.BoardSelection == SettingsCollector.Board.BGT24 || SettingsCollector.BoardSelection == SettingsCollector.Board.RRC160_IntN || SettingsCollector.BoardSelection == SettingsCollector.Board.FindMine;
            bool groupFracN = SettingsCollector.BoardSelection == SettingsCollector.Board.FractionalN_PLL || SettingsCollector.BoardSelection == SettingsCollector.Board.RRC160_FracN;
            bool groupOffsetFrequ = SettingsCollector.BoardSelection == SettingsCollector.Board.RRC160_IntN || SettingsCollector.BoardSelection == SettingsCollector.Board.RRC160_FracN;

            FileInfo FI = new FileInfo(configFilePath);
            
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            
            XmlWriter writer = null;
            try
            {
                if(File.Exists(configFilePath))
                    FI.Attributes &= ~FileAttributes.Hidden; //remove hidden bit to be able to access
                writer = XmlWriter.Create(configFilePath, settings);
            }
            catch (Exception)
            {
                //Can't create file
                return false;
            }

            writer.WriteStartDocument();
            writer.WriteComment("This file is a configuration file for Radar Config an Measurement Tool");
            writer.WriteStartElement("RadarConfigMeasurementTool");
            writer.WriteAttributeString("Module", ((UInt16)SettingsCollector.BoardSelection).ToString());

            writer.WriteStartElement("System");
            writer.WriteAttributeString("startF", num_Sys_startFrequency.Value.ToString());
            writer.WriteAttributeString("slope", num_Sys_slope.Value.ToString());
            if (SettingsCollector.BoardSelection == SettingsCollector.Board.FindMine)
            {
                if(cB_Sys_FindMineX2Multiplier.Checked)
                    writer.WriteAttributeString("f_mult", "2");
                else
                    writer.WriteAttributeString("f_mult", "1");
            }
            else
            {
                writer.WriteAttributeString("f_mult", num_Sys_freqMult.Value.ToString());
            }
            writer.WriteAttributeString("sys_clk", num_Sys_SysClock.Value.ToString());
            writer.WriteAttributeString("sample_clk", num_Sys_SampleClock.Value.ToString());
            writer.WriteAttributeString("f_ext", num_Sys_extFreqInc.Value.ToString());
            if (groupOffsetFrequ)
            {
                writer.WriteAttributeString("f_offset", num_Sys_RRC160_OffsetFrequency.Value.ToString());
            }
            writer.WriteAttributeString("vco_pre", num_Sys_VCOPrescaler.Value.ToString());
            if (groupDDS)
            {
                writer.WriteAttributeString("pll_pre", cB_Sys_PLL_Prescale.SelectedIndex.ToString());
                writer.WriteAttributeString("pll_a_ctr", num_Sys_PLL_A.Value.ToString());
                writer.WriteAttributeString("pll_b_ctr", num_Sys_PLL_B.Value.ToString());
            }
            writer.WriteAttributeString("samples", num_Sys_samples.Value.ToString());
            writer.WriteAttributeString("skip", num_Sys_Skip.Value.ToString());
            writer.WriteAttributeString("FFT_size", num_Sys_FFT.Value.ToString());
            writer.WriteAttributeString("adc_ch", num_Sys_ADC_ch.Value.ToString());
            writer.WriteAttributeString("gain", num_Sys_gain.Value.ToString());
            if(SettingsCollector.BoardSelection != SettingsCollector.Board.RRC160_IntN && SettingsCollector.BoardSelection != SettingsCollector.Board.RRC160_FracN) 
                writer.WriteAttributeString("dac_bias1", num_Sys_bias1.Value.ToString());
            writer.WriteAttributeString("ramps_p_c", num_Sys_ramps.Value.ToString());
            writer.WriteAttributeString("delay", num_Sys_delay.Value.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Window");
            writer.WriteAttributeString("type", cB_Sys_window.SelectedIndex.ToString());
            writer.WriteAttributeString("nwin", num_Sys_Window_PolOrder.Value.ToString());
            writer.WriteAttributeString("a0", tB_Sys_Win_a0.Text);
            writer.WriteAttributeString("a1", tB_Sys_Win_a1.Text);
            writer.WriteAttributeString("a2", tB_Sys_Win_a2.Text);
            writer.WriteAttributeString("a3", tB_Sys_Win_a3.Text);
            writer.WriteAttributeString("a4", tB_Sys_Win_a4.Text);
            writer.WriteAttributeString("a5", tB_Sys_Win_a5.Text);
            writer.WriteAttributeString("a6", tB_Sys_Win_a6.Text);
            writer.WriteAttributeString("a7", tB_Sys_Win_a7.Text);
            writer.WriteAttributeString("a8", tB_Sys_Win_a8.Text);
            writer.WriteAttributeString("a9", tB_Sys_Win_a9.Text);
            writer.WriteEndElement();

            if (SettingsCollector.BoardSelection == SettingsCollector.Board.RRC01)
            {
                writer.WriteStartElement("supplyDAC");
                writer.WriteAttributeString("LNA_gain", num_Sys_RRC01_LNA_gain.Value.ToString());
                writer.WriteAttributeString("TX_power", num_Sys_RRC01_TX_power.Value.ToString());
                writer.WriteAttributeString("Vref_DAC", num_Sys_RRC01_Vref_DAC.Value.ToString());
                writer.WriteAttributeString("Vcc_RX", num_Sys_RRC01_Vcc_RX.Value.ToString());
                writer.WriteAttributeString("Vcc_RX6", num_Sys_RRC01_Vcc_RX6.Value.ToString());
                writer.WriteAttributeString("Vcc_TX3", num_Sys_RRC01_Vcc_TX3.Value.ToString());
                writer.WriteAttributeString("Vcc_TX2", num_Sys_RRC01_Vcc_TX2.Value.ToString());
                writer.WriteEndElement();
            }

            if (groupDDS)
            {
                writer.WriteStartElement("PLL");
                writer.WriteAttributeString("Ref", lbl_ADF4108_R.Text);
                writer.WriteAttributeString("AB", lbl_ADF4108_AB.Text);
                writer.WriteAttributeString("Function", lbl_ADF4108_Func.Text);
                writer.WriteEndElement();

                writer.WriteStartElement("DDS");
                writer.WriteAttributeString("Control", lbl_AD5930_Control.Text);
                writer.WriteAttributeString("FSTART_lsb", lbl_AD5930_FSTART_lsb.Text);
                writer.WriteAttributeString("FSTART_msb", lbl_AD5930_FSTART_msb.Text);
                writer.WriteAttributeString("DeltaF_lsb", lbl_AD5930_DeltaF_lsb.Text);
                writer.WriteAttributeString("DeltaF_msb", lbl_AD5930_DeltaF_msb.Text);
                writer.WriteAttributeString("Ninc", lbl_AD5930_Ninc.Text);
                writer.WriteAttributeString("tINT", lbl_AD5930_tINT.Text);
                writer.WriteAttributeString("TBURST", lbl_AD5930_TBURST.Text);
                writer.WriteEndElement();
            }

            if (groupFracN)
            {
                writer.WriteStartElement("HMC");
                writer.WriteAttributeString("r01", lbl_HMC_RST.Text);
                writer.WriteAttributeString("r02", lbl_HMC_REFDIV.Text);
                writer.WriteAttributeString("r03", lbl_HMC_FRegInt.Text);
                writer.WriteAttributeString("r04", lbl_HMC_FRegFrac.Text);
                writer.WriteAttributeString("r05", lbl_HMC_Seed.Text);
                writer.WriteAttributeString("r06", lbl_HMC_SdCfg.Text);
                writer.WriteAttributeString("r07", lbl_HMC_LockD.Text);
                writer.WriteAttributeString("r08", lbl_HMC_AnalogEn.Text);
                writer.WriteAttributeString("r09", lbl_HMC_ChargeP.Text);
                writer.WriteAttributeString("r10", lbl_HMC_ModStep.Text);
                writer.WriteAttributeString("r11", lbl_HMC_PD.Text);
                writer.WriteAttributeString("r12", lbl_HMC_ALTINT.Text);
                writer.WriteAttributeString("r13", lbl_HMC_ALTFRAC.Text);
                writer.WriteAttributeString("r15", lbl_HMC_GPO.Text);

                writer.WriteEndElement();
            }
            else if (SettingsCollector.BoardSelection == SettingsCollector.Board.BGT24)
            {
                writer.WriteStartElement("BGT24");
                writer.WriteAttributeString("control", lbl_BGT_control.Text);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();

            writer.Flush();
            writer.Close();

            if (hidden)
            {
                FI.Attributes = FileAttributes.Hidden;
            }

            return true;
        }
    }
}
