using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using EH.CRC;
using System.Runtime.InteropServices;
using Radar_Config_and_Measurement_Tool;

namespace EH.DSP
{
    class DSP_Comm
    {
        private SerialPort port;
        private Int16 tout;

        private static UInt16 id_ctr = 1;

        struct DSP_command
        {
            public UInt16 id;
            public UInt16 cmd_ans;
            public UInt32 count;
            public UInt32 addr;
            public UInt16 crc1;
            public UInt16[] data;
            public UInt16 crc2;
        }

        private DSP_command tx_cmd;
        private DSP_command rx_cmd;



        public DSP_Comm(Int16 timeoutInS = 5)
        {
            tout = (Int16)(timeoutInS *20);

            Random rnd = new Random();
            id_ctr = (UInt16)rnd.Next(1, 1000); // creates a number between 1 and 1000
        }

        ~DSP_Comm()
        {
            closeCOM();
        }

        #region Commu
        public bool openCOM(string portname)
        {
            closeCOM();
            port = new SerialPort(portname, 1000000, Parity.None, 8, StopBits.One);
            try
            {
                port.Open();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public void closeCOM()
        {
            if (port != null && port.IsOpen)
                port.Close();
        }

        private void transmitData(byte[] data, int length)
        {
            //seperate to nibbles + 65
            byte[] dataNibbles = new byte[length * 2];
            for(int i = 0; i< length; i++)
            {
                dataNibbles[2 * i] = (byte)((data[i] & 0xF) + 65);
                dataNibbles[2 * i + 1] = (byte)((data[i] >> 4) + 65);
            }
            port.ReadExisting();
            port.Write(dataNibbles, 0, length * 2);
        }

        private bool sendCommand(UInt16 cmd, UInt16[] txData, UInt32 txDataLength, out UInt16[] rxData, out UInt32 rxDataLength, 
            out UInt16 answerCode)
        {
            rxData = null;
            rxDataLength = 0;
            answerCode = 0;

            if (txDataLength > 0 && txData.Length < txDataLength)
            {
                return false;
            }

            tx_cmd.id = id_ctr++;
            tx_cmd.cmd_ans = cmd;
            tx_cmd.count = txDataLength;
            tx_cmd.addr = 0xFFFFFFFF;       //Old format???

            //fill CRC1
            CRC_DSP.CRC16_init();
            CRC_DSP.CRC16_nextData(tx_cmd.id);
            CRC_DSP.CRC16_nextData(tx_cmd.cmd_ans);
            CRC_DSP.CRC16_nextData((UInt16)(tx_cmd.count & 0xFFFF));
            CRC_DSP.CRC16_nextData((UInt16)(tx_cmd.count >> 16));
            CRC_DSP.CRC16_nextData((UInt16)(tx_cmd.addr & 0xFFFF));
            tx_cmd.crc1 =  CRC_DSP.CRC16_nextData((UInt16)(tx_cmd.addr >> 16));

            int tx_length = 14; //Only Overhead: 2 Byte ID, 2 Byte Cmd, 4 Byte Count, 4 Byte Addr, 2 byte CRC
            if (txDataLength > 0)
            {
                tx_cmd.data = new UInt16[txDataLength];
                Array.Copy(txData, tx_cmd.data, txDataLength);

                //fill CRC2
                CRC_DSP.CRC16_init();
                for (int i = 0; i < txDataLength; i++)
                {
                    tx_cmd.crc2 = CRC_DSP.CRC16_nextData(tx_cmd.data[i]);
                }

                tx_length += (int)txDataLength * 2 + 2; //+2 because of CRC
            }

            //convertToByteArray
            byte[] data = new byte[tx_length];
            data[0] = (byte)tx_cmd.id;
            data[1] = (byte)(tx_cmd.id >> 8);
            data[2] = (byte)tx_cmd.cmd_ans;
            data[3] = (byte)(tx_cmd.cmd_ans >> 8);
            data[4] = (byte)tx_cmd.count;
            data[5] = (byte)(tx_cmd.count >> 8);
            data[6] = (byte)(tx_cmd.count >> 16);
            data[7] = (byte)(tx_cmd.count >> 24);
            data[8] = (byte)tx_cmd.addr;
            data[9] = (byte)(tx_cmd.addr >> 8);
            data[10] = (byte)(tx_cmd.addr >> 16);
            data[11] = (byte)(tx_cmd.addr >> 24);
            data[12] = (byte)tx_cmd.crc1;
            data[13] = (byte)(tx_cmd.crc1 >> 8);

            if (txDataLength > 0)
            {
                for (int i = 0; i < txDataLength; i++)
                {
                    data[14 + i * 2] = (byte)tx_cmd.data[i];
                    data[15 + i * 2] = (byte)(tx_cmd.data[i] >> 8);
                }
                data[14 + 2 * txDataLength] = (byte)tx_cmd.crc2;
                data[14 + 2 * txDataLength + 1] = (byte)(tx_cmd.crc2 >> 8);
            }

            //Transmit
            transmitData(data, tx_length);

            //Read response
            int t = tout;
            byte idx = 0;
            bool finished = false;

            byte[] rx_data = new byte[14]; //Only Overhead: 2 Byte ID, 2 Byte Cmd, 4 Byte Count, 4 Byte Addr, 2 byte CRC (as nibbles => *2)
            
            //read Header
            while (!finished && t > 0)
            {
                if(port.BytesToRead >= 14)
                {
                    for (int i = 0; i < 14; i++)
                    {
                        rx_data[idx] = (byte)port.ReadByte();
                        idx++;
                    }
                    finished = true;
                }
                else
                {
                    System.Threading.Thread.Sleep(50);
                    t--;
                }
            }

            if (t == 0)
            {
                return false;
            }

            rx_cmd.id = (UInt16)((rx_data[1] << 8) + rx_data[0]);
            answerCode = rx_cmd.cmd_ans = (UInt16)((rx_data[3] << 8) + rx_data[2]);
            rxDataLength = rx_cmd.count = (UInt32)((rx_data[7] << 24) + (rx_data[6] << 16) + (rx_data[5] << 8) + rx_data[4]);
            rx_cmd.addr = (UInt32)((rx_data[11] << 24) + (rx_data[10] << 16) + (rx_data[9] << 8) + rx_data[8]);
            rx_cmd.crc1 = (UInt16)((rx_data[13] << 8) + rx_data[12]);

            if (rx_cmd.id != tx_cmd.id)
            {
                return false;
            }
            CRC_DSP.CRC16_init();
            CRC_DSP.CRC16_nextData(rx_cmd.id);
            CRC_DSP.CRC16_nextData(rx_cmd.cmd_ans);
            CRC_DSP.CRC16_nextData((UInt16)(rx_cmd.count & 0xFFFF));
            CRC_DSP.CRC16_nextData((UInt16)(rx_cmd.count >> 16));
            CRC_DSP.CRC16_nextData((UInt16)(rx_cmd.addr & 0xFFFF));
            UInt16 crc_tmp = CRC_DSP.CRC16_nextData((UInt16)(rx_cmd.addr >> 16));
            if (crc_tmp != rx_cmd.crc1)
            {
                return false;
            }

            //read data
            if (rxDataLength > 0)
            {
                rx_cmd.data = new UInt16[rxDataLength];

                finished = false;
                while (!finished && t > 0)
                {
                    if (port.BytesToRead >= (rxDataLength*2 + 2))
                    {
                        for(int i = 0; i < rxDataLength; i++)
                        {
                            rx_cmd.data[i] = (byte)port.ReadByte();
                            rx_cmd.data[i] = (UInt16)(rx_cmd.data[i] + (port.ReadByte() <<8));
                        }
                        rx_cmd.crc2 = (byte)port.ReadByte();
                        rx_cmd.crc2 = (UInt16)(rx_cmd.crc2 + (port.ReadByte() << 8));

                        finished = true;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(250);
                        t--;
                    }
                }

                //check crc2
                CRC_DSP.CRC16_init();
                for (int i = 0; i < rxDataLength; i++)
                {
                    crc_tmp = CRC_DSP.CRC16_nextData(rx_cmd.data[i]);
                }

                if (crc_tmp != rx_cmd.crc2)
                {
                    return false;
                }
                //copy data
                rxData = new UInt16[rxDataLength];
                Array.Copy(rx_cmd.data, rxData, rxDataLength);
            }

            System.Threading.Thread.Sleep(50);

            if (t == 0)
                return false;

            return true;
        }
        #endregion

        #region ReadData and FFT Params (<100)
        public bool pingBoard() //Cmd: 0
        {
            UInt16[] rxData;
            UInt32 rxCount;
            UInt16 answerCode;

            if (!sendCommand(0, null, 0, out rxData, out rxCount, out answerCode))
            {
                return false;
            }

            return true;
        }

        public bool getBoardModuleAndVersion(out char module, out UInt16 version) //Cmd: 10
        {
            UInt16[] rxData;
            UInt32 rxCount;
            UInt16 answerCode;

            module = '-';
            version = 0;

            if (!sendCommand(10, null, 0, out rxData, out rxCount, out answerCode))
            {
                return false;
            }

            if (answerCode != 0)
            {
                return false;
            }

            if (rxCount >= 2)
            {
                module = (char)rxData[0];
                version = rxData[1];
            }
            else
            {
                return false;
            }

            return true;
        }

        public bool setFFT_Params(UInt16 samples, UInt16 skip_samples, UInt16 nfft, UInt16 stages, UInt16 rampsPerCycle, UInt16 Delay_btw_ramps) //Cmd: 21
        {
            UInt16[] txData = { samples, skip_samples, nfft, stages, rampsPerCycle, Delay_btw_ramps };
            UInt16[] rxData;
            UInt32 rxCount;
            UInt16 answerCode;

            if (!sendCommand(21, txData, 6, out rxData, out rxCount, out answerCode))
            {
                return false;
            }

            return true;
        }

        public bool calcTwiddleFactors() //Cmd: 23
        {
            UInt16[] rxData;
            UInt32 rxCount;
            UInt16 answerCode;

            if (!sendCommand(23, null, 0, out rxData, out rxCount, out answerCode))
            {
                return false;
            }

            return true;
        }

        public bool readSampling_Buffer(UInt16 ramp_num, out UInt16[] zf_data, out UInt32 length) //Cmd: 27
        {
            UInt16[] txData = { ramp_num };
            UInt16 answerCode;

            if (!sendCommand(27, txData, 1, out zf_data, out length, out answerCode))
            {
                return false;
            }

            return true;
        }
        
        public bool readZF_Buffer(out UInt16[] zf_data, out UInt32 length) //Cmd: 31
        {
            UInt16 answerCode;

            if (!sendCommand(31, null, 0, out zf_data, out length, out answerCode))
            {
                return false;
            }

            return true;
        }

        public bool readMag_Buffer1(out float[] mag_data, out UInt32 length) //Cmd: 32
        {
            length = 0;
            mag_data = null;
            UInt16 answerCode;
            UInt32 rx_length;
            UInt16[] rxData;

            if (!sendCommand(32, null, 0, out rxData, out rx_length, out answerCode))
            {
                return false;
            }

            //reinterpret as float
            length = rx_length / 2;
            mag_data = new float[length];

            for (int i = 0; i < length; i++)
            {
                //byte[] binaryFloat = {(byte)(rxData[2*i+1]>>8), (byte)rxData[2*i+1], (byte)(rxData[2*i]>>8), (byte)rxData[2*i]};
                byte[] binaryFloat = { (byte)rxData[2 * i], (byte)(rxData[2 * i] >> 8), (byte)rxData[2 * i + 1], (byte)(rxData[2 * i + 1] >> 8) };
                mag_data[i] = BitConverter.ToSingle(binaryFloat, 0);
            }


            return true;
        }
        #endregion

        #region DAC bias (100)
        public bool setDACBias(UInt16 biasNr, UInt16 bias1) //Cmd: 106
        {
            UInt16[] txData = { biasNr, bias1, 0 };
            UInt16[] rxData;
            UInt32 rxCount;
            UInt16 answerCode;

            if (!sendCommand(106, txData, 3, out rxData, out rxCount, out answerCode))
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Measurement (200)
        public bool multiSweepWithTimeAveraging() //Cmd: 202
        {
            UInt16[] rxData;
            UInt32 rxCount;
            UInt16 answerCode;

            if (!sendCommand(202, null, 0, out rxData, out rxCount, out answerCode))
            {
                return false;
            }

            return true;
        }

        public bool multiSweepWithoutEvaluation() //Cmd: 203
        {
            UInt16[] rxData;
            UInt32 rxCount;
            UInt16 answerCode;

            if (!sendCommand(203, null, 0, out rxData, out rxCount, out answerCode))
            {
                return false;
            }

            return true;
        }

        public bool readActualGain(out UInt16 act_gain) //Cmd: 207
        {
            act_gain = 0;

            UInt16[] rxData;
            UInt32 rxCount;
            UInt16 answerCode;

            if (!sendCommand(207, null, 0, out rxData, out rxCount, out answerCode))
            {
                return false;
            }

            act_gain = answerCode;

            return true;
        }

        public bool evalTimeAveraging() //Cmd: 208
        {
            UInt16[] rxData;
            UInt32 rxCount;
            UInt16 answerCode;

            if (!sendCommand(208, null, 0, out rxData, out rxCount, out answerCode))
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Power Control (300)
        public bool setPowerMode(UInt16 powerMode) //Cmd: 300
        {
            UInt16[] txData = { powerMode };
            UInt16[] rxData;
            UInt32 rxCount;
            UInt16 answerCode;

            if (!sendCommand(300, txData, 1, out rxData, out rxCount, out answerCode))
            {
                return false;
            }

            return true;
        }

        public bool checkBufferFull(out bool buffEmpty) //Cmd: 301
        {
            buffEmpty = false;

            UInt16[] rxData;
            UInt32 rxCount;
            UInt16 answerCode;

            if (!sendCommand(301, null, 0, out rxData, out rxCount, out answerCode))
            {
                return false;
            }

            if (answerCode != 0)
                buffEmpty = true;

            return true;
        }

        public bool AD5305_DAC(UInt16 DAC_B, UInt16 DAC_C, UInt16 DAC_D) //Cmd: 304
        {
            UInt16[] txData = { DAC_B, DAC_C, DAC_D };
            UInt16[] rxData;
            UInt32 rxCount;
            UInt16 answerCode;

            if (!sendCommand(304, txData, 3, out rxData, out rxCount, out answerCode))
            {
                return false;
            }

            return true;
        }
        #endregion

        #region TMP100 (305)
        public bool readTemp(out float temperature) //Cmd: 305
        {
            temperature = 0;

            UInt16[] rxData;
            UInt32 rxCount;
            UInt16 answerCode;

            if (!sendCommand(305, null, 0, out rxData, out rxCount, out answerCode))
            {
                return false;
            }

            if (rxCount >= 1)
            {
                temperature = rxData[0];
                temperature /= 100;
            }
            else
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Resoulution data (400)
        public bool setResolution(float dR, float df) //Cmd: 403
        {
            byte[] dR_bytes = BitConverter.GetBytes(dR);
            byte[] df_bytes = BitConverter.GetBytes(df);

            UInt16[] txData = { (UInt16)((dR_bytes[1] << 8) + dR_bytes[0]), 
                                  (UInt16)((dR_bytes[3] << 8) + dR_bytes[2]), 
                                  (UInt16)((df_bytes[1] << 8) + df_bytes[0]), 
                                  (UInt16)((df_bytes[3] << 8) + df_bytes[2]) };
            UInt16[] rxData;
            UInt32 rxCount;
            UInt16 answerCode;

            if (!sendCommand(403, txData, 4, out rxData, out rxCount, out answerCode))
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Window (500)
        public bool setWindowParams(UInt16 windowSize, float[] windowParameter) //Cmd: 500
        {
            UInt16[] txData = { windowSize,0, 
                                  0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 };
            
            int length = windowParameter.Length;
            if(length > 10)
                length = 10;
            for(int i=0; i<length; i++)
            {
                byte[] p_bytes = BitConverter.GetBytes(windowParameter[i]);

                txData[i*2 + 2] = (UInt16)((p_bytes[1] << 8) + p_bytes[0]);
                txData[i*2 + 2 + 1] = (UInt16)((p_bytes[3] << 8) + p_bytes[2]);
            }

            UInt16[] rxData;
            UInt32 rxCount;
            UInt16 answerCode;

            if (!sendCommand(500, txData, 22, out rxData, out rxCount, out answerCode))
            {
                return false;
            }
            
            return true;
        }
        public bool calcWindow() //Cmd: 501
        {
            UInt16[] rxData;
            UInt32 rxCount;
            UInt16 answerCode;

            if (!sendCommand(501, null, 0, out rxData, out rxCount, out answerCode))
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Board specific (>1000)
        public bool AD5930_DDS_Registers(UInt16 control, UInt16 f_start_lsb, UInt16 f_start_msb, UInt16 delta_f_lsb, 
            UInt16 delta_f_msb, UInt16 Ninc, UInt16 tINT, UInt16 TBurst) //Cmd: 1002
        {
            UInt16[] txData = { control, f_start_lsb, f_start_msb, delta_f_lsb, delta_f_msb, Ninc, tINT, TBurst };
            UInt16[] rxData;
            UInt32 rxCount;
            UInt16 answerCode;

            if (!sendCommand(1002, txData, 8, out rxData, out rxCount, out answerCode))
            {
                return false;
            }

            return true;
        }

        public bool ADF4108_PLL_Registers(UInt32 funtion, UInt32 R, UInt32 AB) //Cmd: 4001
        {
            UInt16[] txData = { (UInt16)(funtion & 0xFFFF), (UInt16)(funtion >> 16), (UInt16)(R & 0xFFFF), (UInt16)(R >> 16), 
                                  (UInt16)(AB & 0xFFFF), (UInt16)(AB >> 16)};
            UInt16[] rxData;
            UInt32 rxCount;
            UInt16 answerCode;

            if (!sendCommand(4001, txData, 6, out rxData, out rxCount, out answerCode))
            {
                return false;
            }

            return true;
        }

        public bool HMC703_Registers(UInt32 id, UInt32 rst, UInt32 refdiv, UInt32 freqInt, UInt32 freqFrac, UInt32 seed, UInt32 sd_cfg, 
            UInt32 lockDetect, UInt32 analogEn, UInt32 chargePump, UInt32 modulation, UInt32 pd, UInt32 altInt, UInt32 altFrac, 
            UInt32 spiTrig, UInt32 gpo) //Cmd: 4001
        {
            UInt16[] txData = { (UInt16)(id & 0xFFFF), (UInt16)(id >> 16),
                                  (UInt16)(rst & 0xFFFF), (UInt16)(rst >> 16),
                                  (UInt16)(refdiv & 0xFFFF), (UInt16)(refdiv >> 16), 
                                  (UInt16)(freqInt & 0xFFFF), (UInt16)(freqInt >> 16), 
                                  (UInt16)(freqFrac & 0xFFFF), (UInt16)(freqFrac >> 16), 
                                  (UInt16)(seed & 0xFFFF), (UInt16)(seed >> 16), 
                                  (UInt16)(sd_cfg & 0xFFFF), (UInt16)(sd_cfg >> 16), 
                                  (UInt16)(lockDetect & 0xFFFF), (UInt16)(lockDetect >> 16), 
                                  (UInt16)(analogEn & 0xFFFF), (UInt16)(analogEn >> 16), 
                                  (UInt16)(chargePump & 0xFFFF), (UInt16)(chargePump >> 16), 
                                  (UInt16)(modulation & 0xFFFF), (UInt16)(modulation >> 16), 
                                  (UInt16)(pd & 0xFFFF), (UInt16)(pd >> 16), 
                                  (UInt16)(altInt & 0xFFFF), (UInt16)(altInt >> 16), 
                                  (UInt16)(altFrac & 0xFFFF), (UInt16)(altFrac >> 16), 
                                  (UInt16)(spiTrig & 0xFFFF), (UInt16)(spiTrig >> 16), 
                                  (UInt16)(gpo & 0xFFFF), (UInt16)(gpo >> 16) };
            UInt16[] rxData;
            UInt32 rxCount;
            UInt16 answerCode;

            if (!sendCommand(4001, txData, 32, out rxData, out rxCount, out answerCode))
            {
                return false;
            }

            return true;
        }

        public bool ADC_Settings(UInt16 samples, UInt16 channel, UInt16 gain) //Cmd: 5000
        {
            UInt16[] txData = { samples,channel,gain };
            UInt16[] rxData;
            UInt32 rxCount;
            UInt16 answerCode;

            if (!sendCommand(5000, txData, 3, out rxData, out rxCount, out answerCode))
            {
                return false;
            }

            return true;
        }

        public bool FindMine_EnableX2Line(bool enabled) //Cmd: 6001
        {
            UInt16[] txData = { 0 };
            if (enabled)
                txData[0] = 1;
            UInt16[] rxData;
            UInt32 rxCount;
            UInt16 answerCode;

            if (!sendCommand(6001, txData, 1, out rxData, out rxCount, out answerCode))
            {
                return false;
            }

            return true;
        }

        public bool evalE_DAC(UInt16 supplyA, UInt16 supplyB, UInt16 supplyC, UInt16 supplyD, UInt16 CtrlA, UInt16 CtrlB, UInt16 CtrlC, UInt16 CtrlD) //Cmd: 6100 + 6101
        {
            UInt16[] txData = { supplyA, supplyB, supplyC, supplyD };
            UInt16[] rxData;
            UInt32 rxCount;
            UInt16 answerCode;

            if (!sendCommand(6100, txData, 4, out rxData, out rxCount, out answerCode))
            {
                return false;
            }

            UInt16[] txData2 = { CtrlA, CtrlB, CtrlC, CtrlD };

            if (!sendCommand(6101, txData2, 4, out rxData, out rxCount, out answerCode))
            {
                return false;
            }

            return true;
        }

        public bool BGT24MTR11_Control(UInt16 control) //Cmd: 6101
        {
            UInt16[] txData = { control };
            UInt16[] rxData;
            UInt32 rxCount;
            UInt16 answerCode;

            if (!sendCommand(6101, txData, 1, out rxData, out rxCount, out answerCode))
            {
                return false;
            }

            return true;
        }
        #endregion

        #region BGT24 AUX ADC (120)
        public bool readBGT24AuxADC(out UInt16 channel0, out UInt16 channel1) //Cmd: 120
        {
            channel0 = channel1 = 0;

            UInt16[] rxData;
            UInt32 rxCount;
            UInt16 answerCode;

            if (!sendCommand(120, null, 0, out rxData, out rxCount, out answerCode))
            {
                return false;
            }

            if (rxCount >= 2)
            {
                channel0 = rxData[0];
                channel1 = rxData[1];
            }
            else
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Combined communication
        public bool performRamp(UInt16 ramp_num, out UInt16[] zf_data, out UInt32 zf_length, out float[] mag_data, out UInt32 mag_length, out float temperature, out UInt16 gain)
        {
            bool successful = readTemp(out temperature);
            successful &= setPowerMode(1);

            bool bufferEmpty;
            do
            {
                successful &= checkBufferFull(out bufferEmpty);
                System.Threading.Thread.Sleep(100);
            } while (bufferEmpty);

            successful &= setPowerMode(0);

            successful &= multiSweepWithoutEvaluation();
            successful &= evalTimeAveraging();
            successful &= readActualGain(out gain);
            if (ramp_num == 0)
            { //Average
                successful &= readZF_Buffer(out zf_data, out zf_length);
            }
            else
            {
                ramp_num--;
                successful &= readSampling_Buffer(ramp_num, out zf_data, out zf_length);
            }
            successful &= readMag_Buffer1(out mag_data, out mag_length);

            return successful;
        }
        #endregion

    }
}
