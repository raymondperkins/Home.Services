using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ServiceStack;

namespace Home.Models
{
    [Route("/controls")]
    public class ControlsModel
    {
        public string Filter { get; set; }

    }

    [Route("/controls/Id")]
    public class ControlModel
    {
        public string Id { get; set; }

    }
}
