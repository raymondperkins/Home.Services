using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ServiceStack;

namespace Home.Models
{
    [Route("/schedules")]
    public class SchedulesModel
    {
        public string Filter { get; set; }

    }

    [Route("/schedules/{Id}")]
    public class ScheduledModel
    {
        public string Id { get; set; }

    }
}
