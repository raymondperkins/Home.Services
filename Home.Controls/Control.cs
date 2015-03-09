using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Home.Services
{
    /// <summary>
    /// Base control class where type T is the config for the control
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Control<T> :  IControl
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public object GetConfig()
        {
            return GetControlConfig();
        }

        public void SetConfig(object value)
        {
            SetControlConfig((T)value);
        }

        /// <summary>
        /// Sets the enabled or disabled state of the controller
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(bool value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the enabled or disabled state of the controller
        /// </summary>
        /// <returns></returns>
        public bool GetValue()
        {
            return Value;
        }

        /** control implementation **/
        public bool Value
        {
            get { return GetControlValue(); }
            set { SetControlValue(value); }
        }


        public T Config
        {
            get { return GetControlConfig(); }
            set { SetControlConfig(value); }
        }


        private T _config;
        public virtual T GetControlConfig()
        {
            if (_config == null) _config = default(T);
            return _config;
        }

        public virtual void SetControlConfig(T value)
        {
            _config = value;
        }



        public virtual bool GetControlValue()
        {
            return false;
        }

        public virtual void SetControlValue(bool value)
        {

        }

        public Control()
        {
            this.Type = typeof(T).Name;
        }

    }
}
