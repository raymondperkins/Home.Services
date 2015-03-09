using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Home.Services
{
    public static class SensorHomeExtensions
    {
        static SensorHomeExtensions()
        {

        }

        static Sensors _instanceSensors = null;
        public static Sensors InstanceSensors
        {
            get { return _instanceSensors ?? (_instanceSensors= new Sensors()); }
            set { _instanceSensors = value; }
        }

        public static Sensors GetSensors(this App instance)
        {
            return InstanceSensors;
        }

        public static void InitSensors(this App instance)
        {
            Sensors sensors = InstanceSensors;
            string deviceUrl = App.GetDeviceUrl();
            // setup uPnp for schedular
            UpnpService service = new UpnpService(
                "home-services-sensors",
                "home_services_sensors",
                deviceUrl,
                deviceUrl,
                deviceUrl
                );
            App.AddUpnpService(service);
            App.AddServiceRoute(new PageRoute() { href = "/sensors", img = "/images/Ethernet-Cable-icon.png", alt = "View sensors" });
        }
    }
}
