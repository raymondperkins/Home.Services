using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ServiceStack;

namespace Home.Models
{
    [Route("/sensing")]
    public class SensingModel
    {
        public string Filter { get; set; }
    }

    [Route("/sensing/{Id}")]
    public class SensorModel
    {
        public string Id { get; set; }
    }
}
