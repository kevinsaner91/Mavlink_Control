using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace EH.RadarControl
{
    class UniUlm_PositionControl : PositionControl
    {
        UInt32 ref_distance;
        double ref_stepsize;

        public UniUlm_PositionControl(double stepsize, UInt32 maxDistance, bool enableDebugOutput = false, Int16 timeout = 200)
            : base(timeout, enableDebugOutput)
        {
            ref_distance = maxDistance;
            ref_stepsize = stepsize;
        }

        public override bool openCOM(string portName)
        {
            return base.internal_openCOM(portName, 9600, Parity.None, 8, StopBits.One);
        }

        public bool setZeroPoint()
        {
            port.ReadExisting();
            printDebugMessage("Send data: @0n1", "Motor:setZeroPoint");
            string data = "@0n1\r";
            port.Write(data);

            return true;
        }

        public override bool referenceDrive()
        {
            setSpeed(5000);

            port.ReadExisting();
            printDebugMessage("Send data: @0R1", "Motor:referenceDrive");
            string data = "@0R1\r";
            port.Write(data);

            int t = tout;
            int rx;
            while ((rx = port.ReadByte()) != 48 && t > 0)
            {
                printDebugMessage("Read Byte: " + rx.ToString(), "Motor:referenceDrive");
                System.Threading.Thread.Sleep(250);
                t--;
            }

            if (t == 0)
            {
                return false;
            }

            setZeroPoint();
            return true;
        }

        public override void setSpeed(UInt16 speed)
        {
            if (speed > 10000)
                speed = 10000;

            port.ReadExisting();
            printDebugMessage("Send data: @0d" + speed.ToString(), "Motor:setSpeed");
            string data = "@0d" + speed.ToString() + "\r";
            port.Write(data);

            port.ReadByte();
        }

        public override bool setPosition(UInt32 position)
        {
            return setPosition(position, 5000);
        }

        public bool setPosition(UInt32 position, UInt16 speed)
        {
            if (position > ref_distance)
                position = ref_distance;
            if (speed > 10000)
                speed = 10000;
            UInt32 distance = (UInt32)(position * ref_stepsize);
            port.ReadExisting();
            printDebugMessage("Send data: @0M" + distance.ToString() + "," + speed.ToString(), "Motor:setPosition");
            string data = "@0M" + distance.ToString() + "," + speed.ToString() + "\r";
            port.Write(data);

            int t = tout;
            int rx;
            while ((rx=port.ReadByte()) != 48 && t > 0)
            {
                printDebugMessage("Read Byte: " + rx.ToString(), "Motor:setPosition");
                System.Threading.Thread.Sleep(250);
                t--;
            }

            if (t == 0)
            {
                return false;
            }
            printDebugMessage("Read Byte: " + rx.ToString(), "Motor:setPosition");
            return true;
        }

        public UInt32 getPosition()
        {
            UInt32 distance = 0;
            port.ReadExisting();
            printDebugMessage("Send data: @0P", "Motor:getPosition");
            string data = "@0P\r";
            port.Write(data);
            System.Threading.Thread.Sleep(250);
            char[] value = new char[7];
            for (int i = 0; i <= 6; i++)
            {
                value[i] = (char)port.ReadChar();
            }

            string st_data = new string(value);

            distance = Convert.ToUInt32(st_data, 16);

            distance = (UInt32)(distance / ref_stepsize);

            printDebugMessage("Read data: " + distance.ToString(), "Motor:getPosition");

            return distance;
        }
    }
}
