using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Threading;

using ServiceStack.Logging;
using ServiceStack.Text;
using Raspberry.IO.GeneralPurpose;

using Home.Services;

using Home;

namespace Home.Control.Server
{
    class Program
    {
        private static System.Timers.Timer updateTimer = null;

        private static string appname { get { return "Control Server"; } }
        private static string appversion { get { return "1.16"; } }
        private static ControlServerConfig config;

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            
            Console.WriteLine("Starting {0}, version {1}", appname, App.ApplicationVersion);
            /*
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach(var iface in interfaces)
            {
                Console.WriteLine("Net Interface {0} status {2}, {1}", iface.Name, iface.Description, iface.OperationalStatus);

            }
            */
            LogManager.LogFactory = new ConsoleLogFactory();

            

            App.AllowConsoleWriteline = true;
            App.Instance.InitSchedular();
            App.Instance.InitControls();
            App.Instance.InitSensors();
            //App.StartInstance();
            var controls = App.Instance.GetControls();
            var schedular = App.Instance.GetSchedular();

            // load settings
            config = ControlServerConfig.Load();
            /*
            foreach (var control in config.Controls)
                controls.Add(control);

            foreach (var schedule in config.Schedules)
                schedular.Add(schedule);
            */
            App.Instance.ServiceItemChanged += (s, e) =>
            {
                // trigger a save to the config.
                config.Save();
            };

            schedular.StartSchedular();

            if (App.IsUnix == true)
            {
               
                // load the controls
                controls.Add(new TemperaturePifaceOutputControl(
                    new TemperaturePifaceOutputControlConfig { Inverted = false, GPIO = "5 6 7", TempSensorId = "28-0000042dd80d" }) { Id = "temp1", Name = "Piface temperature control" });  
                /*
                controls.Add(new OutputControl(new IOControlConfig { Inverted = false, GPIO = "17" }) { Id = "gpio17", Name = "GPIO17" });                
                controls.Add(new OutputControl(new IOControlConfig { Inverted = false, GPIO = "21" }) { Id = "gpio21", Name = "GPIO21" });
                controls.Add(new OutputControl(new IOControlConfig { Inverted = false, GPIO = "22" }) { Id = "gpio22", Name = "GPIO22" });
                controls.Add(new OutputControl(new IOControlConfig { Inverted = false, GPIO = "23" }) { Id = "gpio23", Name = "GPIO23" });
                */
                // add scheduler items
                DateTime timeStart = DateTime.Now.AddSeconds(1);
                /*
                schedular.AddSchedule(new OutputControlSchedule("gpio17")
                {
                    ActiveSeconds = 3,
                    ReoccurSeconds = 3,
                    Id = "gpio17Reoccur",
                    Name = "gpio17Reoccur",
                    From = timeStart,
                });
                 */
            }
            else
            {
                // load bogus controls
                controls.Add(new TemperaturePifaceOutputControl(
                    new TemperaturePifaceOutputControlConfig { Inverted = false, GPIO = "5 6 7", TempSensorId = "28-0000042dd80d" }) 
                    { Id = "temp1", Name = "Piface temperature control" });  
                
                controls.Add(new TestOutputControl(new IOControlConfig { Inverted = false, GPIO = "17" }) { Id = "gpio17", Name = "GPIO17" });  
                /*
                 * controls.Add(new TestOutputControl(new IOControlConfig { Inverted = false, GPIO = "21" }) { Id = "gpio21", Name = "GPIO21" });
                controls.Add(new TestOutputControl(new IOControlConfig { Inverted = false, GPIO = "22" }) { Id = "gpio22", Name = "GPIO22" });
                */
                // add scheduler items
                DateTime timeStart = DateTime.Now.AddSeconds(1);
                
                schedular.AddSchedule(new ControlSchedule("gpio17")
                    {
                        ActiveSeconds = 3,
                        ReoccurSeconds = 3,
                        Id = "gpio17Reoccur",
                        Name = "gpio17Reoccur",
                        From = timeStart,
                    });
                 
            }

            // Start web server
            var appHost = new AppHost();
            appHost.Init();
            appHost.Start(string.Format("http://*:{0}/", App.GetDeviceHttpPort()));
            //appHost.Start(Home.GetDeviceUrl());

            string.Format("Listening on {0}...", App.GetDeviceUrl()).Print(); ;
            "Press any key to quit..".Print();
            App.WaitForExit();       
            appHost.Stop();
            
            App.Instance.Stop();
            schedular.StopSchedular();
            // clean up pins
            foreach (IControl cntrl in controls)
            {
                cntrl.SetValue(false);
                if (cntrl is IDisposable) { (cntrl as IDisposable).Dispose(); }
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
            {
                try
                {
                    foreach (IControl cntrl in App.Instance.GetControls())
                    {
                        cntrl.SetValue(false);
                        if (cntrl is IDisposable) { (cntrl as IDisposable).Dispose(); }
                    }
                }
                catch { }
            }
            
            Console.Write(e.ToString());
        }

        private static void RPiGPIONetTest(ProcessorPin pinVal)
        {
            ProcessorPin pin = pinVal;
            int loopCount = 20;
            IGpioConnectionDriver gpio = new FileGpioConnectionDriver()
            {

            };
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            gpio.Allocate(pin, PinDirection.Output);
            for (int i = 0; i < 10; i++)
            {
                gpio.Write(pin, true);
                Console.WriteLine("SET: TRUE,  PIN STATE: {0}   TIME: {1}", gpio.Read(pin), stopwatch.ElapsedTicks);
                Thread.Sleep(500);
                gpio.Write(pin, false);
                Console.WriteLine("SET: FALSE, PIN STATE: {0}  TIME: {1}", gpio.Read(pin), stopwatch.ElapsedTicks);
                Thread.Sleep(500);
            }

            Console.WriteLine("Doing loop read test");
            while (loopCount-- >= 0)
            {
                gpio.Write(pin, true);
                Console.WriteLine("SET: TRUE,  PIN STATE: {0}   TIME: {1}", gpio.Read(pin), stopwatch.ElapsedTicks);
                gpio.Write(pin, false);
                Console.WriteLine("SET: FALSE, PIN STATE: {0}  TIME: {1}", gpio.Read(pin), stopwatch.ElapsedTicks);
                //HighResolutionTimer.Sleep(0.100m);
            }
            gpio.Write(pin, true);

            gpio.Release(pin);
            stopwatch.Stop();
        }
        
        /*
        static void Main(string[] args)
        {
            Console.WriteLine("Starting {0}, version {1}", appname, appversion);
            int hostPort = Environment.OSVersion.ToString().Contains("Unix") ? 80 : 2001;
            Uri deviceUri = new Uri(string.Format("http://{0}{1}", GetMyIp(), hostPort != 80 ? (":" + hostPort.ToString()) : ""));
            if (AppContext.Current.IsUnixHost) Console.WriteLine("Hosted on Unix so GPIO access will be allowed");
            else Console.WriteLine("Not a Unix host so GPIO will not be used");
            Console.WriteLine("Host {0} started at {1} on {2}", deviceUri.ToString(), DateTime.Now.ToString("yyyyMMdd HH:mm:ss"), Environment.OSVersion);

            LogManager.LogFactory = new ConsoleLogFactory();
            AppContext.EnableConsoleWriteline = true;
            Licenses.RegisterServiceStackLicense();
            
            // add dummy controllers
            AppContext.Current.AddController(new Home.Common.Control.TestOutputController("001"));
            AppContext.Current.AddController(new Home.Common.Control.TestOutputController("002"));
            AppContext.Current.AddController(new Home.Common.Control.TestOutputController("003"));
            //pout.Enable();

            
            updateTimer = new System.Timers.Timer(1000); // update every .5 second
            
            // If this app is running on a Unix, then assume it is a linux OS on a Pi that has a PiFace connected
            // Otherwise this will crash in windows due to the lack of libc
            if (Environment.OSVersion.ToString().Contains("Unix"))
            {

                //pout = new Common.Control.PinOutputController("27", true);
                pout = new Common.Control.TempOutputController("28-0000042dd80d", "27", true)
                {
                    TemperatureSetpoint = 15.0,
                    TemperatureMargin = 2.0
                };
                var eventItem = new Home.Common.ControllerScheduleItem()
                {
                    Name = "Test Event 1",
                    ControllerUid = pout.Uid,
                    EventEnableDateTime = DateTime.Now.AddSeconds(4),
                    ReoccurSeconds = 20,
                    EventLengthSeconds = 360,
                    IsEnabled = true,
                };
                //Home.Common.AppContext.Current.Schedular.AddEvent(eventItem);
                AppContext.Current.AddController(pout);
                pout.Enable();
                updateTimer.Elapsed += new System.Timers.ElapsedEventHandler(updateTimer_Elapsed);
                //updateTimer.Start();
                AppContext.WriteLine("{0} vs {1}", eventItem.ControllerUid, pout.Uid);
                
            }

            var appHost = new AppHost();
            appHost.Init();
            appHost.Start(string.Format("http://*:{0}/", hostPort));
            

            string.Format("\n\nListening on http://*:{0}/", hostPort).Print();

            // create upnp server
            //var controller = CreateServiceController();

            Mono.Upnp.Root root = new Mono.Upnp.Root(
                new Mono.Upnp.DeviceType("schemas-upnp-org", "mono-home-controller-pi", new Version(1, 0)),
                "uuid:2fac1234-31f8-11b4-a222-08002b34c003",
                "Garage LED device",
                "Raymond Perkins",
                "Home Controller",                
                new DeviceOptions() {
                    SerialNumber = "TestSN",
                    Upc = "RPiRev2",
                    PresentationUrl = deviceUri,
                    Services = new Mono.Upnp.Service[] { },
                    EmbeddedDevices = new Mono.Upnp.Device[] { },
                }
            );
            
            var server = new Mono.Upnp.Server(root);
            //Console.WriteLine("Starting UPNP server");
            //server.Start();
             


            "Press any key to quit..".Print();
            
            Console.ReadLine();//Thread.Sleep(Timeout.Infinite);
            if(updateTimer.Enabled) updateTimer.Stop();
            Home.Common.AppContext.Current.Schedular.Stop();
            appHost.Stop();
            server.Stop();
            server.Dispose();
            AppContext.Current.DisposeControllers();            
        }
        */

        static void updateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            
        }

        static string GetMyIp()
        {
            string str = string.Empty;
      
            System.Net.IPHostEntry ips = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
      
            foreach (System.Net.IPAddress ip in ips.AddressList)
            {
                Console.WriteLine(ip.ToString());
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    str = ip.ToString();
                    break;
                }
            }
            if (string.IsNullOrEmpty(str)) str = "127.0.0.1";
            return str;
        }
    }
}
