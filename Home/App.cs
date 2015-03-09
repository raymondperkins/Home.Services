using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Reflection;
namespace Home
{
    public class App
    {
        public static string CommonFolder { get{ return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),"homeservices"); }}

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        List<PageRoute> _routes;
        public  List<PageRoute> Routes { get { return _routes; } }

        private UpnpManager upnpManager;

        public event EventHandler<ServiceItemChangedEventArgs> ServiceItemChanged;
        public event EventHandler<ServiceItemValueChangedEventArgs> ServiceItemValueChanged;

        public UpnpManager UPnP { get { return upnpManager; } }

        public App()
        {
            upnpManager = IsUnix ? new UpnpManager() : null;
            _routes = new List<PageRoute>();
            Debug("Home instance created");

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //throw new NotImplementedException();
            
        }

        public void Start()
        {
            if (upnpManager != null)
            {
                upnpManager.StartServer();
            }
        }
        public void Stop()
        {
            if (upnpManager != null)
            {
                upnpManager.StopServer();
            }
        }

        public void AddRoute(IEnumerable<PageRoute> routes)
        {
            foreach (var route in routes)
                AddRoute(route);
        }

        public void AddRoute(PageRoute route)
        {
            if (_routes.Count == 0 || _routes.TrueForAll(q => q.href == route.href) == false)
                _routes.Add(route);
            else
                Debug("failed to add page route: {0}", route.href);
        }

        /// <summary>
        /// Called by a service controller when one of its items has changed.
        /// </summary>
        /// <param name="item">item that was changed</param>
        public void OnServiceInstanceItemChanged(object item)
        {
            if(ServiceItemChanged != null)
            {
                ServiceItemChanged(this, new ServiceItemChangedEventArgs() { Item = item } );
            }
        }

        /// <summary>
        /// Called by a service controller when one the values of an item has changed.
        /// The value is either the input, output or schedule item active states.
        /// </summary>
        /// <param name="item">item that was changed</param>
        public void OnServiceInstanceItemValueChanged(object item)
        {
            if (ServiceItemValueChanged != null)
            {
                ServiceItemValueChanged(this, new ServiceItemValueChangedEventArgs() { Item = item });
            }
        }



        #region Static helpers

        private static App appinstance = null;
        public static App Instance { get { return appinstance ?? (appinstance = new App()); } }

        public static void StartInstance()
        {
            Instance.Start();
        }

        public static void AddUpnpService(UpnpService service)
        {
            if (Instance.UPnP != null)
                Instance.UPnP.Add(service);
        }

        public static void AddServiceRoute(PageRoute route)
        {
            Instance.AddRoute(route);
        }

        public static bool IsUnix { get { return Environment.OSVersion.ToString().Contains("Unix"); } }

        public static string ApplicationVersion 
        { 
            get
            {
                var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
                return fvi.FileVersion;
            }
        }

        public static void WaitForExit()
        {
            // wait for exit
            using (EventWaitHandle cancelWaithandle = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                Console.CancelKeyPress += (s, e) =>
                {
                    cancelWaithandle.Set();
                };
                cancelWaithandle.WaitOne();
            }
            
        }

        public static int GetDeviceHttpPort()
        {
            return IsUnix ? 80 : 2001;
        }

        public static string GetDeviceIp()
        {
            string str = string.Empty;

            System.Net.IPHostEntry ips = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());

            foreach (System.Net.IPAddress ip in ips.AddressList)
            {
                Debug(ip.ToString());
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    str = ip.ToString();
                    break;
                }
            }
            if (string.IsNullOrEmpty(str)) str = "127.0.0.1";
            return str;
        }

        public static string GetDeviceBaseUrl()
        {
            return string.Format("http://{0}", App.GetDeviceIp());
        }

        public static string GetDeviceBaseUrl(string format)
        {
            return string.Format("{0}{1}", GetDeviceBaseUrl(), format);
        }

        public static string GetDeviceUrl()
        {
            return string.Format("http://{0}{1}/",
                App.GetDeviceIp(),
                App.GetDeviceHttpPort() != 80 ? (":" + App.GetDeviceHttpPort().ToString()) : "");
        }

        public static string GetAppHostUrl()
        {
            return string.Format("http://{0}:{1}/",
                "*",
                App.GetDeviceHttpPort());
        }

        /// <summary>
        /// Settung to true will allow the WriteLine method to print to the console
        /// </summary>
        public static bool AllowConsoleWriteline { get { return _printToConsole; } set { _printToConsole = value; } }
        private static bool _printToConsole = false;
        /// <summary>
        /// Writes a line to the console if print is enabled
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void WriteLine(string format, params object[] args)
        {
            if (_printToConsole == true)
            {
                Console.WriteLine(format, args);
            }
        }

        
        public static void Debug(string format, params object[] args)
        {
            WriteLine(format, args);
        }

        #endregion
    }
}
