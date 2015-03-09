using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Home.Services;

namespace Home.Control.Server
{
    public class ControlServerConfig : SettingsFile
    {
        public List<IControl> Controls { get; set; }

        public List<ControlSchedule> Schedules { get; set; }


        public ControlServerConfig()
        {
            Controls = new List<IControl>();
            Schedules = new List<ControlSchedule>();
        }

        public static ControlServerConfig Load()
        {
            var config = SettingsFile.Load<ControlServerConfig>();
            // init defaults if need be
            if (config.Controls.Count == 0)
            {
                // add configs
                if (App.IsUnix == true)
                    config.Controls.Add(new OutputControl(new IOControlConfig { Inverted = false, GPIO = "17" }) { Id = "gpio17", Name = "GPIO17" });    
                else
                    config.Controls.Add(new TestOutputControl(new IOControlConfig { Inverted = false, GPIO = "17" }) { Id = "gpio17", Name = "GPIO17" });    
            }
            if (config.Schedules.Count == 0)
            {
                config.Schedules.Add(new ControlSchedule("gpio17")
                {
                    ActiveSeconds = 3,
                    ReoccurSeconds = 3,
                    Id = "gpio17Schedule",
                    Name = "gpio17Reoccur",
                    From = DateTime.Now.AddSeconds(1),
                });
            }

            return config;
        }


    }
}
