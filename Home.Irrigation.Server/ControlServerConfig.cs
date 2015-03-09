using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Home.Services;

namespace Home.Irrigation.Server
{
    [Serializable]
    public class IrrigationServerConfig : SettingsFile
    {
        public List<IControl> Controls { get; set; }

        public List<ISchedule> Schedules { get; set; }


        public IrrigationServerConfig()
        {
            Controls = new List<IControl>();
            Schedules = new List<ISchedule>();
        }

        public static IrrigationServerConfig Load()
        {
            var config = SettingsFile.Load<IrrigationServerConfig>();
            // init defaults if need be

            return config;
        }


    }
}
