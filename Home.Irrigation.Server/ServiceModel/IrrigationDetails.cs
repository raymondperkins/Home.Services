using System;
using System.Collections.Generic;

using ServiceStack;
using ServiceStack.Text;

using Home.Irrigation.Server;

namespace Home.Control.ServiceModel
{

    [Route("/piface/state", "GET")]
    public class PiFaceState
    {

    }

    [Route("/piface/state/{OutputNumber}/enable", "POST")]
    public class PiFaceEnableOutput
    {
        public string OutputNumber { get; set; }
    }

        [Route("/piface/state/{OutputNumber}/disable", "POST")]
    public class PiFaceDisableOutput
    {
        public string OutputNumber {get; set;}
    }

    public class PiFaceChangeOutputResponse
    {
        public bool Success { get; set; }
        public bool NewValue { get; set; }
    }

    public class PiFaceStateResponse
    {
        public DateTime Time { get; set; }
        public string TimeString { get { return Time.ToString("yyyy/MM/dd HH:mm:ss"); } }
        public PifacePinStates PinStates { get; set; }
    }

    public class TurbineLogRecord
    {
        public DateTime Time { get; set; }
        public string TimeString { get { return Time.ToString("yyyy/MM/dd HH:mm:ss"); } }
        public double Frequency { get; set; }
        public double Voltage { get; set; }
        public double Current { get; set; }
        public double Power { get; set; }
    }
}
