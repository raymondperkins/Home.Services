using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Home.Services;

namespace Home.Irrigation.Server
{

    public class TestOutputControl : Control<IOControlConfig>
    {
        bool controlValue = false;
        IOControlConfig config = null;

        public TestOutputControl(IOControlConfig Config)
        {
            config = Config;
        }

        public override void SetControlValue(bool value)
        {
            controlValue = value;
        }

        public override bool GetControlValue()
        {
            return controlValue;
        }

        public override IOControlConfig GetControlConfig()
        {
            return config;
        }

        public override void SetControlConfig(IOControlConfig value)
        {
            //throw new Exception("This config is not allowed to be changed");
        }
    }
}
