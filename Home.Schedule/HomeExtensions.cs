using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Home.Services
{
    public static class ScheduleHomeExtensions
    {
        static ScheduleHomeExtensions()
        {

        }


        static Schedular _instanceSchedular = null;
        public static Schedular InstanceSchedular
        {
            get { return _instanceSchedular ?? (_instanceSchedular = new Schedular()); }
            set { _instanceSchedular = value; }
        }

        public static Schedular GetSchedular(this App instance)
        {
            return InstanceSchedular;
        }

        public static void AddSchedule(this App instance, ISchedule item)
        {
            InstanceSchedular.AddSchedule(item);
        }

        public static void InitSchedular(this App instance)
        {            
            Schedular schedular = InstanceSchedular;
            string deviceUrl = App.GetDeviceUrl();
            // setup uPnp for schedular
            UpnpService service = new UpnpService(
                "home-services-schedular",
                "home_services_schedular",
                "schedular",
                "schedular/scpd",
                "schedular/event"
                );
            App.AddUpnpService(service);
            App.AddServiceRoute(new PageRoute() { href = "/schedular", img = "/images/schedule-icon.png", alt = "View schedules" });
        }
    }
}
