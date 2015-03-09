using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Home.Services
{
    public class Sensor<T> :  ISensor
    {
        private string _type = null;
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get { return _type ?? (_type = this.GetType().Name);} set { _type = value; } }
        public string Unit { get; set; }
        public T Value
        {
            get { return GetValue(); }
        }


        public virtual T GetValue()
        {
            return default(T);
        }
    }
}
