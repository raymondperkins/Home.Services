using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Raspberry.IO.GeneralPurpose;

namespace Home.Irrigation.Server
{
    public class PiFaceIOControlConfig
    {
        // if set then the pin state is inverted
        public bool Inverted { get; set; }
        public int Pin { get; set; }

        public ProcessorPin ConvertStringToPin(string value)
        {
            value = value.ToLower();
            int pinVal = 0;
            if (value.StartsWith("p1"))
            {
                value = value.Substring(2, value.Length - 2).Trim(new char[] { ' ', '-', '_' });
                int.TryParse(value, out pinVal);
            }
            else if (value.StartsWith("p5"))
            {
                value = value.Substring(2, value.Length - 2).Trim(new char[] { ' ', '-', '_' });
                int.TryParse(value, out pinVal);
                pinVal += 100;
            }
            else if (int.TryParse(value, out pinVal))
            {
                if (pinVal >= 0 && pinVal <= 30)
                    return ((ProcessorPin)pinVal);
            }
            switch (pinVal)
            {
                case 3: return ConnectorPin.P1Pin03.ToProcessor();
                case 5: return ConnectorPin.P1Pin05.ToProcessor();
                case 7: return ConnectorPin.P1Pin07.ToProcessor();
                case 8: return ConnectorPin.P1Pin08.ToProcessor();
                case 10: return ConnectorPin.P1Pin10.ToProcessor();
                case 11: return ConnectorPin.P1Pin11.ToProcessor();
                case 12: return ConnectorPin.P1Pin12.ToProcessor();
                case 13: return ConnectorPin.P1Pin13.ToProcessor();
                case 15: return ConnectorPin.P1Pin15.ToProcessor();
                case 16: return ConnectorPin.P1Pin16.ToProcessor();
                case 18: return ConnectorPin.P1Pin18.ToProcessor();
                case 19: return ConnectorPin.P1Pin19.ToProcessor();
                case 21: return ConnectorPin.P1Pin21.ToProcessor();
                case 22: return ConnectorPin.P1Pin22.ToProcessor();
                case 23: return ConnectorPin.P1Pin23.ToProcessor();
                case 24: return ConnectorPin.P1Pin24.ToProcessor();
                case 26: return ConnectorPin.P1Pin26.ToProcessor();
                // P5 header
                case 103: return ConnectorPin.P5Pin03.ToProcessor();
                case 104: return ConnectorPin.P5Pin04.ToProcessor();
                case 105: return ConnectorPin.P5Pin05.ToProcessor();
                case 106: return ConnectorPin.P5Pin06.ToProcessor();
                default:
                    break;
            }

            throw new Exception("Unable to convert string to ConnectorPin");
        }
    }
}
