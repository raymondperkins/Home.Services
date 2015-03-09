using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Home.Services
{
    public class Schedular : List<ISchedule>
    {
        private Task _schedularThread = null;
        private int schedularThreadLoopDelayMs = 650;
        private int removeCompleteScheduleDelayS = 3600; // 1 hour
        private object _writeLock = new object();
        private bool canRun = false;

        public int SchedularThreadLoopDelayMs { get { return schedularThreadLoopDelayMs; } set { schedularThreadLoopDelayMs = value; } }
        public int RemoveCompleteScheduleDelaySeconds { get { return removeCompleteScheduleDelayS; } set { removeCompleteScheduleDelayS = value; } }

        public bool IsRunning { get { return _schedularThread == null ? false : !_schedularThread.IsCompleted; } }

        new public void Add(ISchedule item)
        {
            AddSchedule(item);
        }

        new public void Remove(ISchedule item)
        {
            RemoveSchedule(item);
        }
        
        public void AddSchedule(ISchedule item)
        {
            // check item doesnt exist already
            ISchedule existing = this.FirstOrDefault(q => q.Id == item.Id);
            if (existing == null)
            {
                lock (_writeLock)
                {
                    base.Add(item);
                }
                OnScheduleAdded(item);
            }
        }

        public void RemoveSchedule(ISchedule item)
        {
            // check item doesnt exist already
            ISchedule existing = this.FirstOrDefault(q => q.Id == item.Id);
            if (existing != null)
            {
                lock (_writeLock)
                {
                    base.Remove(existing);
                }
                OnScheduleRemoved(existing);
            }
        }

        public void StartSchedular()
        {
            if (_schedularThread == null || _schedularThread.IsCompleted)
            {
                canRun = true;
                _schedularThread = Task.Factory.StartNew(() => SchedularThread());
                _schedularThread.ContinueWith((task) =>
                    {
                        App.Debug("Schedular stopped");
                        if (task.IsFaulted)
                        {
                            App.Debug("Schedular task has faulted:");
                            foreach (var ex in task.Exception.InnerExceptions)
                                App.Debug(ex.ToString());
                        }
                    });
            }
        }

        public bool StopSchedular()
        {
            canRun = false;
            return _schedularThread == null ? true : _schedularThread.Wait(1500);
        }

        /** internal helpers **/

        private void SchedularThread()
        {
            App.Debug("Schedular started");
            while (canRun)
            {
                List<ISchedule> items = new List<ISchedule>();

                lock (_writeLock)
                {
                    items.AddRange(this);
                }
                
                //
                foreach (ISchedule item in items)
                {
                    DateTime now = DateTime.Now;
                    // if item should not be running
                    if (!item.CanRun && item.IsActive)
                    {
                        StopScheduleItem(item);
                    }

                    // if item should be running
                    if (item.From < now && now < item.To) 
                    {
                        // check that it is allowed to start
                        if(item.CanRun)
                            StartScheduleItem(item);
                    }
                    // if item should be stopped
                    else if (item.IsActive)//now >= item.To)
                    {
                        // stop item and then check to see if it needs to be reset to
                        // a new time. This is indicated by the reoccurseconds property.
                        StopScheduleItem(item);
                        if (item.ReoccurSeconds < 1)
                        {
                            // item will not reoccur and should be removed from sch.
                            if ((now - item.To).TotalSeconds >= removeCompleteScheduleDelayS)
                            {
                                // remove
                                RemoveSchedule(item);
                            }
                        }
                        else
                        {
                            while (item.From < now)
                                item.From = item.From.AddSeconds(item.ReoccurSeconds);
                        }
                    }
                    // if item time to run has not happened yet
                    else
                    {

                    }
                }

                // delay loop;
                Thread.Sleep(schedularThreadLoopDelayMs);
            }
        }

        private void StartScheduleItem(ISchedule item)
        {
            // check if the item is running, if not try to start it.
            if (item.IsActive == false)
            {
                try
                {
                    item.SetOn();
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void StopScheduleItem(ISchedule item)
        {
            // check if the item is running and try to stop it
            if (item.IsActive == true)
            {
                try
                {
                    item.SetOff();
                }
                catch (Exception ex)
                {

                }
            }
        }


        private void OnScheduleAdded(ISchedule item)
        {

        }

        private void OnScheduleRemoved(ISchedule item)
        {
            StopScheduleItem(item);
        }
    }
}
