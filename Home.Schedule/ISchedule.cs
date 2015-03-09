using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Home.Services
{
    public interface ISchedule
    {
        string Id { get; set; }
        string Name { get; set; }
        string Type { get; set; }
        bool CanRun { get; set; }
        bool IsActive { get; set; }
        double ReoccurSeconds { get; set; }
        double ActiveSeconds { get; set; }
        DateTime From { get; set; }
        DateTime To { get; set; }


        void SetOn();
        void SetOff();
    }
}
