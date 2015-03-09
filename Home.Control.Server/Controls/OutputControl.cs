using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Raspberry.IO.GeneralPurpose;

using Home.Services;

namespace Home.Control.Server
{

    public class OutputControl : Control<IOControlConfig>, IDisposable
    {
        IOControlConfig config = null;

        public OutputControl(IOControlConfig Config)
        {
            config = Config;
            // set pin as an output
            var pin = config.Pin;
            GpioConnectionSettings.DefaultDriver.Allocate(pin, PinDirection.Output);
            App.WriteLine("Controls: Allocated output for {0}", pin);
        }

        public override void SetControlValue(bool value)
        {

            var pin = config.Pin;
            GpioConnectionSettings.DefaultDriver.Write(pin, config.Inverted ? !value : value);
            App.WriteLine("Controls: Set output to {0} for {1}", config.Inverted ? !value : value, pin);
        }

        public override bool GetControlValue()
        {
            var pin = config.Pin;
            bool state = GpioConnectionSettings.DefaultDriver.Read(pin);

            return config.Inverted ? !state : state;            
        }

        public void Dispose()
        {
            var pin = config.Pin;
            GpioConnectionSettings.DefaultDriver.Allocate(pin, PinDirection.Input);
        }
    }
}
