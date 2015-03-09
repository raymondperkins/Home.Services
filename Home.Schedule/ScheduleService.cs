using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using ServiceStack;
using ServiceStack.Text;
using Home;

namespace Home.Services
{
    public class ScheduleService : Service, IHomeServiceController
    {
        public object Get(GetSchedules request)
        {
            List<ScheduleModel> response = new List<ScheduleModel>();
            
            foreach (var item in ScheduleHomeExtensions.InstanceSchedular)
            {
                response.Add(ToModel(item));
            }
            
            return response;
        }
        /*
        public object Get(GetSchedule request)
        {
            // find the sensor that matches this Id
            ISchedule item = ScheduleHomeExtensions.InstanceSchedular.FirstOrDefault(q => q.Id == request.Id);
            if (item == null)
            {
                throw HttpError.NotFound(string.Format("{0}", request.Id));
            }

            return ToModel(item);
        }
        */
        public object Get(GetScheduleTemplate request)
        {
            // find the sensor that matches this Id
            ISchedule item = ScheduleHomeExtensions.InstanceSchedular.FirstOrDefault(q => q.Id == request.Id);
            if (item == null)
            {
                throw HttpError.NotFound(string.Format("{0}", request.Id));
            }

            // find the template file for this item.
            string name = item.GetType().Name;
            string basedir = Home.App.AssemblyDirectory;//AssemblyUtils.GetAssemblyBinPath(System.Reflection.Assembly.GetExecutingAssembly());
            //templatedir = Path.Combine(templatedir, "Views", "templates");
            string filePath = Path.Combine(basedir, "Views", "templates", string.Format("{0}.html", name));

            if(File.Exists(filePath))
            {
                // return file
                return File.OpenRead(filePath);
            }
            else
            {
                // return default
                return File.OpenRead(Path.Combine(basedir, "Views", "templates", "scheduleitem.html"));
            }
        }

        public object Put(PutSchedule request)
        {
            // find the sensor that matches this Id
            ISchedule item = ScheduleHomeExtensions.InstanceSchedular.FirstOrDefault(q => q.Id == request.Id);
            if (item == null)
            {
                throw HttpError.NotFound(string.Format("{0}", request.Id));
            }
            item.Name = request.Name;
            item.From = request.From;
            item.To = request.To;
            item.ReoccurSeconds = request.ReoccurSeconds;
            item.CanRun = request.CanRun;
            App.Debug("schedular item {0} new start: {1} end: {2}", item.Id, item.From, item.To);

            // notify Home instance that a change has occured, this is so it can do things like save the instances
            App.Instance.OnServiceInstanceItemChanged(item);
            return ToModel(item);
        }

        private ScheduleModel ToModel(ISchedule item)
        {
            var model = new ScheduleModel()
            {
                Id = item.Id,
                Name = item.Name,
                Type = item.Type,
                ReoccurSeconds = item.ReoccurSeconds,
                ActiveSeconds = item.ActiveSeconds,
                CanRun = item.CanRun,
                IsActive = item.IsActive,
                From = item.From,
                To = item.To,
            };
            return model;
        }
    }
}
