using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace EH.RadarControl
{
    /*
     * There is no laser tracker at Uni Ulm -> just simulate! 
     */
    class UniUlm_PositionTracker : PositionTracker
    {
        private UniUlm_PositionControl control;

        public UniUlm_PositionTracker(UniUlm_PositionControl motor, bool enableDebugOutput = false, Int16 timeout = 1000)
            : base(timeout, enableDebugOutput)
        {
            control = motor;
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
            printDebugMessage("Send data: getPosition", "Tracker:getPosition");
            double retVal = control.getPosition();
            printDebugMessage("Read Distance: " + retVal.ToString(), "Tracker:getPosition");
            return retVal;
        }
    }
}
