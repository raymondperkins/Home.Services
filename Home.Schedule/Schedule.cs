using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Home.Services
{
    public class Schedule : ISchedule
    {
        private bool _canrun = true;
        private bool _isactive = false;

        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public bool CanRun { get { return _canrun; } set { _canrun = value; } }
        public bool IsActive { get { return _isactive; } set { _isactive = value; } }
        public double ReoccurSeconds { get; set; }
        public double ActiveSeconds { get; set; }
        public DateTime From { get; set; }
        public DateTime To
        {
            get
            {
                return From.AddSeconds(ActiveSeconds);
            }
            set
            {
                ActiveSeconds = (value - From).TotalSeconds;
            }
        }


        public void SetOn()
        {
            OnActivated();
            this.IsActive = true;
            
        }

        public void SetOff()
        {
            OnDeactivated();
            this.IsActive = false;
        }

        public virtual void OnActivated()
        {
            
        }

        public virtual void OnDeactivated()
        {

        }
    }
}
