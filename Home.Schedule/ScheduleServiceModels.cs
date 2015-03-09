using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using ServiceStack;

namespace Home.Services
{
    [Route("/schedular")]
    public class GetSchedules : IReturn<List<ScheduleModel>>
    {
        //public List<ISensor> Results { get; set; }
    }


    [Route("/schedular/{Id}", "GET")]
    public class GetSchedule : IReturn<ScheduleModel>
    {
        public string Id { get; set; }
    }

    [Route("/schedular/{Id}/template", "GET")]
    public class GetScheduleTemplate : IReturn<Stream>
    {
        public string Id { get; set; }
    }

    [Route("/schedular/{Id}", "PUT")]
    public class PutSchedule : ScheduleModel
    {

    }
        
    public class ScheduleModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public bool CanRun { get; set; }
        public bool IsActive { get; set; }
        public double ReoccurSeconds { get; set; }
        public double ActiveSeconds { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
