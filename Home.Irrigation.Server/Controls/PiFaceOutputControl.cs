using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Raspberry.IO.GeneralPurpose;

using Home.Services;

namespace Home.Irrigation.Server
{

    public class PiFaceOutputControl : Control<PiFaceIOControlConfig>, IDisposable
    {
        public PiFaceOutputControl(PiFaceIOControlConfig Config)
        {
            this.Config = Config;
            // set pin as an output
        }

        public override void SetControlValue(bool value)
        {
            var pin = Config.Pin;
            AppGlobal.PifaceSetOutputPin(Config.Pin, Config.Inverted ? !value : value);
            App.WriteLine("Controls: Set output to {0} for {1}", Config.Inverted ? !value : value, pin);
        }

        public override bool GetControlValue()
        {
            var pin = Config.Pin;
            bool state = AppGlobal.PiFace.GetOutputPinState((byte)pin);

            return Config.Inverted ? !state : state;            
        }

        public void Dispose()
        {
        }
    }
}
