using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ServiceStack;

namespace Home.Services
{
    [Route("/sensors")]
    public class SensorsModel : Sensors
    {
        //public List<ISensor> Results { get; set; }
    }

    [Route("/sensors/{Id}")]
    public class SensorModel : ISensor
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
