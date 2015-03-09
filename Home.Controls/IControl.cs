using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Home.Services
{
    public interface IControl
    {
        string Id { get; set; }
        string Name { get; set; }
        string Type { get; set; }

        void SetConfig(object value);
        object GetConfig();

        void SetValue(bool value);
        bool GetValue();
    }
}
