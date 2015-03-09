using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mono.Upnp;


namespace Home
{
    public class UpnpService : Service
    {
        public UpnpService(string serviceType, string id, string controlUrl, string scpdUrl, string eventUrl)
            : base(new ServiceType("schemas-upnp-org", serviceType, new Version(1, 0)), 
            string.Format("urn:upnp-org:serviceId:{0}", id), 
            new UpnpServiceController())
        {
            this.ControlUrlFragment = controlUrl;
            this.ScpdUrlFragment = scpdUrl;
            this.EventUrlFragment = eventUrl;
            //this.Type = ServiceType.Parse(serviceType);
        }
    }
}
