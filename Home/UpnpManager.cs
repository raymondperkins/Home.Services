using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Mono.Ssdp;
using Mono.Upnp;
using Mono.Upnp.Control;

namespace Home
{
    public class UpnpManager : List<Service>
    {
        private Root upnpRoot;
        private Server upnpServer;

        public string TypeString { get; set; }
        public string UDN { get; set; }
        public string FriendlyName { get; set; }
        public string Manufactururer { get; set; }
        public string ModelName { get; set; }
        public string SerialNumber { get; set; }
        public string Upc { get; set; }
        public string PresentationUrl { get; set; }

        public UpnpManager()
        {
            // init defaults
            TypeString = "mono-home-controller-pi";
            UDN = "uuid:2fac1234-31f8-11b4-a222-08002b34c003";
            FriendlyName = "Home Services device";
            Manufactururer = "Mono";
            ModelName = "Home Services device";
            SerialNumber = "";
            Upc = "RPiRev2";
            PresentationUrl = App.GetDeviceUrl();            
        }

        public void StartServer()
        {
            if (upnpRoot == null)
            {
                upnpRoot = new Mono.Upnp.Root(
                new Mono.Upnp.DeviceType("schemas-upnp-org", TypeString, new Version(1, 0)),
                UDN,
                FriendlyName,
                Manufactururer,
                ModelName,
                new DeviceOptions()
                {
                    SerialNumber = this.SerialNumber,
                    Upc = this.Upc,
                    PresentationUrl = new Uri(this.PresentationUrl),
                    Services = this,//new Mono.Upnp.Service[] { },
                    EmbeddedDevices = new Mono.Upnp.Device[] { },
                });

                
                upnpServer = new Server(upnpRoot);
            }

            if (upnpServer.Started == false)
            {
                upnpServer.Start();
            }
        }

        public void StopServer()
        {
            if (upnpServer != null && upnpServer.Started == true)
            {
                upnpServer.Stop();
            }
        }

        public void Restart()
        {
            if (upnpServer != null)
            {
                StopServer();
            }
            StartServer();
        }

        new public void Add(Service item)
        {
            base.Add(item);
            //Restart();
        }

        new public void Remove(Service item)
        {
            base.Remove(item);

            //Restart();
        }
    }
}
