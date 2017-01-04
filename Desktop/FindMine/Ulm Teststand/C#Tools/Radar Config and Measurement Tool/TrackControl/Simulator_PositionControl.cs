using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace EH.RadarControl
{
    class Simulator_PositionControl : PositionControl
    {
        public Simulator_PositionControl(bool enableDebugOutput = false, Int16 timeout = 100)
            : base(timeout, enableDebugOutput)
        {
        }

        private static UInt32 actPosition = 0;

        public static UInt32 getPosition()
        {
            return actPosition;
        }

        public override bool openCOM(string portName)
        {
            return true;
        }

        public override void closeCOM()
        {
        }

        public override bool referenceDrive()
        {
            printDebugMessage("Simulate send data: referenceDrive", "Motor:referenceDrive");
            actPosition = 0;
            return true;
        }

        public override void setSpeed(UInt16 speed)
        {
            printDebugMessage("Simulate send data: setSpeed", "Motor:setSpeed");
        }

        public override bool setPosition(UInt32 position)
        {
            printDebugMessage("Simulate send data: setPosition", "Motor:setPosition");
            actPosition = position;
            return true;
        }
    }
}
