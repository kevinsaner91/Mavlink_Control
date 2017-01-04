using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace EH.RadarControl
{
    class MillTrack_PositionControl : PositionControl
    {
        public MillTrack_PositionControl(bool enableDebugOutput = false, Int16 timeout = 100)
            : base(timeout, enableDebugOutput)
        {
        }

        public override bool openCOM(string portName)
        {
            return base.internal_openCOM(portName, 9600, Parity.None, 8, StopBits.One);
        }

        //Not working!!!
        /*public string getS16()
        {
            port.ReadExisting();
            string data = "S016\r\n";
            port.Write(data);
            System.Threading.Thread.Sleep(5000);
            int i = port.BytesToRead;
            return port.ReadExisting();
        }*/

        //Not working!!!
        /*private bool waitForS016Bit(int BitNr)
        {
            bool PH_reached = false;
            int i = 0;
            //System.Threading.Thread.Sleep(100);
            port.ReadExisting();
            do
            {
                string value = getS16();
                int idx = value.IndexOf("S016:");
                if (idx > 0)
                {
                    if (value.Substring(idx + 4 + BitNr, 1).Contains('1'))
                        PH_reached = true;
                }
                i++;
            } while (!PH_reached && i < 100);
            return PH_reached;
        }*/

        public override bool referenceDrive()
        {
            port.ReadExisting();

            printDebugMessage("Send data: ph", "Motor:referenceDrive");

            string data = "ph\r\n";
            port.Write(data);

            printDebugMessage("No further debug messages available -> Waiting for 3 Minutes!", "Motor:referenceDrive");

            //TODO: check if target distance is reached
            //return (waitForS016Bit(4));
            System.Threading.Thread.Sleep(180000); //3 minutes

            return true;
        }

        public override void setSpeed(UInt16 speed)
        {
            if (speed > 100)
                speed = 100;

            port.ReadExisting();

            printDebugMessage("Send data: speed" + speed.ToString(), "Motor:setSpeed");
            string data = "speed" + speed.ToString() + "\r\n";
            port.Write(data);
        }

        public override bool setPosition(UInt32 position)
        {
            if (position < 50)
                position = 50;
            port.ReadExisting();

            printDebugMessage("Send data: pa" + position.ToString(), "Motor:setPosition");

            string data = "pa" + position.ToString() + "\r\n";
            port.Write(data);

            printDebugMessage("No further debug messages available -> wait till laser tracker reached position!", "Motor:setPosition");

            return true;
            //return (waitForS016Bit(5));
        }
    }
}
