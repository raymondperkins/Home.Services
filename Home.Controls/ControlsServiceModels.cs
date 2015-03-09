using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using ServiceStack;

namespace Home.Services
{
    [Route("/controls")]
    public class GetControls : IReturn<List<ControlModel>>
    {
        //public List<ISensor> Results { get; set; }
    }

    [Route("/controls/{Id}", "GET")]
    public class GetControl : ControlModel, IReturn<ControlModel>
    {
    }

    [Route("/controls/{Id}/template", "GET")]
    public class GetScheduleTemplate : IReturn<Stream>
    {
        public string Id { get; set; }
    }

    [Route("/controls/{Id}", "PUT")]
    public class PutControl : ControlModel, IReturn<ControlModel>
    {
    }

    [Route("/controls/{Id}/set")]
    public class PutControlSet : IReturn<ControlModel>
    {
        public string Id { get; set; }
        /// <summary>
        /// Value sets the enabled or disabled state for the control
        /// </summary>
        public bool? Value { get; set; }
    }

    public class ControlModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public object Value { get; set; }
        public object Config { get; set; }
    }

    public class ControlChart
    {
        public int? MaxRecords { get; set; }
        public int? RecordIntervalSec { get; set; }
        public List<ChartJsSeries> SeriesList { get; set; }
    }
}
