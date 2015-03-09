using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Threading;

using ServiceStack;
using ServiceStack.Logging;
using ServiceStack.Text;

using Kingsland.PiFaceSharp.Spi;
using Kingsland.PiFaceSharp.PinControllers;

using Home.Control.ServiceModel;

namespace Home.Irrigation.Server
{
    public static class AppGlobal
    {
        public static string AppPath = @"/home/pi/guruplug/pi/Home Services/Home.TX23ULogger/bin/Debug";

        public static PifacePinStates PiFacePinStates {get; set;}

        /// <summary>
        /// Default piface device
        /// </summary>
        public static PiFaceDevice PiFace = Environment.OSVersion.ToString().Contains("Unix") ? new PiFaceDevice() : null;

        public static PifacePinStates PifaceGetPinInformation()
        {
            
            if(PiFacePinStates == null)
            {
                PiFacePinStates = new PifacePinStates();
                PiFacePinStates.Inputs = new bool[8];
                PiFacePinStates.Outputs = new bool[8];
            }
            return PiFacePinStates;
        }

        public static string PifaceGetPinInformationAsJSON()
        {
            return PifaceGetPinInformation().ToJson();
        }

        public static PifacePinStates PifaceUpdatePinInformation()
        {
            PifacePinStates states = new PifacePinStates();
            if (PiFace != null)
            {
                lock (PiFace)
                {
                    byte inputs = PiFace.GetInputPinStates();
                    byte outputs = PiFace.GetOutputPinStates();
                    states.Inputs = new bool[8];
                    states.Outputs = new bool[8];
                    //Console.WriteLine("In: {0} Out: {1}", inputs, outputs);
                    for (int i = 0; i < 8; i++)
                    {
                        states.Inputs[i] = (inputs & (1 << i)) > 0;
                        states.Outputs[i] = (outputs & (1 << i)) > 0;
                    }
                    PiFacePinStates = states;
                }
            }
            return PiFacePinStates;
        }

        /// <summary>
        /// Thread safe method to set the output pins for the PiFace.
        /// </summary>
        /// <param name="number">Zero based index for the pin to change</param>
        /// <param name="value">True to set pin or false to unset</param>
        /// <returns>The state of the pin after being set</returns>
        public static bool PifaceSetOutputPin(int number, bool value)
        {
            bool state = false;
            if (number > 7) throw new Exception("Number if higher then available outputs");
            if (PiFace != null)
            {
                lock (PiFace)
                {
                    //Console.WriteLine("Setting pin {0}:{1}", number, value);
                    PiFace.SetOutputPinState((byte)number, value);
                    state = PiFace.GetOutputPinState((byte)number);
                }
            }
            return state;
        }
    }


    public class PifacePinStates
    {
        public bool[] Inputs { get; set; }
        public bool[] Outputs { get; set; }
    }
}
