using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Home.Services
{
    public static class ControlHomeExtensions
    {
        static ControlHomeExtensions()
        {

        }

        static Controls _instanceControls = null;
        public static Controls InstanceControls
        {
            get { return _instanceControls ?? (_instanceControls = new Controls()); }
            set { _instanceControls = value; }
        }

        public static Controls GetControls(this App instance)
        {
            return InstanceControls;
        }

        public static IControl GetControlByName(this App instance, string name)
        {
            return InstanceControls.Where(q => q.Name == name).FirstOrDefault();
        }

        public static void InitControls(this App instance)
        {
            Controls controls = InstanceControls;
            string deviceUrl = App.GetDeviceUrl();
            // setup uPnp for schedular
            UpnpService service = new UpnpService(
                "home-services-controls",
                "home_services_controls",
                deviceUrl,
                deviceUrl,
                deviceUrl
                );
            App.AddUpnpService(service);
            App.AddServiceRoute(new PageRoute() { href = "/controls", img = "/images/Setting-Gear-Icon.png", alt = "View controls" });
        }
    }
}
