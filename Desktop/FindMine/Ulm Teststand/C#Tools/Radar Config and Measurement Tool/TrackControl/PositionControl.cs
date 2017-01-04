using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.IO;

namespace EH.RadarControl
{
    abstract class PositionControl
    {
        protected Int16 tout;
        protected SerialPort port;
        protected bool debugOutputEnabled;

        public PositionControl(Int16 timeout, bool enableDebugOutput)
        {
            tout = timeout;
            debugOutputEnabled = enableDebugOutput;
        }

        public string[] getPorts()
        {
            return SerialPort.GetPortNames();
        }

        protected void printDebugMessage(string text, string method)
        {
            if (debugOutputEnabled)
            {
                using (StreamWriter sw = new StreamWriter("motor.dbg", true))
                {
                    sw.WriteLine(DateTime.Now.ToString() + " - " + method + " - " + text);
                }
            }
        }

        public abstract bool openCOM(string portName);

        protected bool internal_openCOM(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            closeCOM();
            port = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
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

        public virtual void closeCOM()
        {
            if (port != null && port.IsOpen)
                port.Close();
        }

        public abstract bool referenceDrive();
        public abstract void setSpeed(UInt16 speed);
        public abstract bool setPosition(UInt32 position);
    }
}
