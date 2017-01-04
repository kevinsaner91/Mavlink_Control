using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace EH.RadarControl
{
    class Lab_PositionTracker : PositionTracker
    {
        double ref_distance;

        public Lab_PositionTracker(bool enableDebugOutput = false, Int16 timeout = 1000)
            : base(timeout, enableDebugOutput)
        {
            ref_distance = 2688 + 269.56;
        }

        public override bool openCOM(string portName)
        {
            return base.internal_openCOM(portName, 9600, Parity.Even, 7, StopBits.One);
        }

        public override double getPosition()
        {
            port.ReadExisting();

            printDebugMessage("Send data: \\411:8\\5", "Tracker:getPosition");
            char[] data = new char[8];
            data[0] = (char)4;
            data[1] = '1';
            data[2] = '1';
            data[3] = ':';
            data[4] = '8';
            data[5] = (char)5;
            data[6] = '\r';
            data[7] = '\n';

            port.Write(data,0,8);

            System.Threading.Thread.Sleep(250);

            char readByte;
            char[] recv = new char[100];
            int i = 0;
            do{
                readByte = recv[i] = (char)port.ReadByte();
                i++;
            }while(readByte != 0x3 && i<100);

            string tmp = new string(recv,4,i-1-4);

            double distance = ref_distance - Convert.ToDouble(tmp) / 100;

            printDebugMessage("Read Distance: " + distance.ToString(), "Tracker:getPosition");

            return distance;
        }
    }
}
