using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Home
{
    public class AppContext
    {
        public bool IsUnixHost { get { return Environment.OSVersion.ToString().Contains("Unix"); } }
        
        private string _appfolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), AppName);
        public string AppFolder { get { return _appfolder; } set { 
            _appfolder = value;
            if (System.IO.Directory.Exists(_appfolder) == false)
                System.IO.Directory.CreateDirectory(_appfolder);
        } }

        /// <summary>
        /// The Name of the EXE that is running as the AppContext
        /// </summary>
        public static string AppName { 
            get 
            { 
                string loc = System.Reflection.Assembly.GetEntryAssembly().Location; 
                if(string.IsNullOrEmpty(loc))
                    return "default";
                else
                    return System.IO.Path.GetFileNameWithoutExtension(loc);
            } 
        }
        private static AppContext _appContext;

        public static AppContext Current { get { if (_appContext == null) { _appContext = new AppContext(); } return _appContext; } }

        /// <summary>
        /// Settung to true will allow the WriteLine method to print to the console
        /// </summary>
        public static bool EnableConsoleWriteline { get { return _printToConsole; } set { _printToConsole = value; } }
        private static bool _printToConsole = false;
        /// <summary>
        /// Writes a line to the console if print is enabled
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void WriteLine(string format, params object[] args)
        {
            if(_printToConsole == true)
            {
                Console.WriteLine(format, args);
            }
        }
    }
}
