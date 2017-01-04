using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace EH.RadarControl
{
    class Simulator_PositionTracker : PositionTracker
    {

        public Simulator_PositionTracker(bool enableDebugOutput = false, Int16 timeout = 1000)
            : base(timeout, enableDebugOutput)
        {
        }

        public override bool openCOM(string portName)
        {
            return true;
        }

        public override void closeCOM()
        {
        }

        public override double getPosition()
        {
            printDebugMessage("Simulate send data: getPosition", "Tracker:getPosition");
            
            UInt32 data = Simulator_PositionControl.getPosition();

            printDebugMessage("Read Distance: " + data.ToString(), "Tracker:getPosition");

            return data;
        }
    }
}
