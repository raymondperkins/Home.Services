using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;

namespace Home
{
    [Serializable]
    public class SettingsFile
    {
        private static string _settingsFolder = App.CommonFolder;
        private const string _filenameFormat = "{0}.json";
        /// <summary>
        /// Set or get the Directory where settings files are stored. This must be set before any log files are created or they will be placed in the default directory.
        /// </summary>
        public static string Folder
        {
            get
            {
                if (System.IO.Directory.Exists(_settingsFolder) == false)
                    System.IO.Directory.CreateDirectory(_settingsFolder);
                return _settingsFolder;
            }
            set
            {
                _settingsFolder = value;
                if (System.IO.Directory.Exists(_settingsFolder) == false)
                    System.IO.Directory.CreateDirectory(_settingsFolder);
            }
        }

        public void Save()
        {
            Directory.CreateDirectory(App.CommonFolder);
            string path = Path.Combine(App.CommonFolder, string.Format(_filenameFormat, this.GetType().Name));
            SeralizeObjectToFile(this, path);
            App.Debug("Saved config to {0}", path);
        }

        public static T Load<T>() where T : new()
        {
            string path = Path.Combine(App.CommonFolder, string.Format(_filenameFormat, typeof(T).Name));         
            try
            {
                if (File.Exists(path))
                {
                    T val = (T)DeseralizeObjectFromFile(typeof(T), path);
                    if (val == null) throw new Exception(string.Format("Failed to load Object type {0} from filepath {1}", typeof(T).Name, path));
                    
                    return val;
                }
            }
            catch(Exception ex)
            {
                App.Debug("Load config error: ", ex.Message);
            }

            return new T();//(T)new SettingsFile();
        }


        public static void SeralizeObjectToFile(object obj, string filePath)
        {
            using (var file = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                ServiceStack.Text.JsonSerializer.SerializeToStream(obj, file);
                //new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Serialize(file, obj);
            }
        }

        public static object DeseralizeObjectFromFile(Type type, string filePath)
        {
            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return ServiceStack.Text.JsonSerializer.DeserializeFromStream(type, file);
                //return new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Deserialize(file);
            }
        }
    }
}