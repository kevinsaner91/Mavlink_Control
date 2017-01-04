using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace EH.RadarControl
{
    class MillTrack_PositionTracker : PositionTracker
    {

        public MillTrack_PositionTracker(bool enableDebugOutput = false, Int16 timeout = 1000)
            : base(timeout, enableDebugOutput)
        {
        }

        public override bool openCOM(string portName)
        {
            return base.internal_openCOM(portName, 9600, Parity.None, 8, StopBits.One);
        }

        public override double getPosition()
        {
            port.ReadExisting();

            printDebugMessage("Send data: start", "Tracker:getPosition");

            string data = "start\r\n";
            port.Write(data);

            System.Threading.Thread.Sleep(500);

            char readByte = 'x';
            char[] recv = new char[100];
            int i = 0, j = 0;
            do{
                if (port.BytesToRead > 0)
                {
                    readByte = recv[i] = (char)port.ReadByte();
                    i++;
                }
                else
                {
                    j++;
                    if (j > 3)
                        readByte = '\0';
                    System.Threading.Thread.Sleep(50);
                }
                
            }while(readByte != '\0' && i<100);

            string tmp = new string(recv);
            tmp = tmp.Replace(',', '.');

            double retVal = 0;
            try
            {
                retVal = Convert.ToDouble(tmp);
            }
            catch (Exception)
            {
                return 99999.99;
            }

            printDebugMessage("Read Distance: " + retVal.ToString(), "Tracker:getPosition");

            return retVal;
        }
    }
}
