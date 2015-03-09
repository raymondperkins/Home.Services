using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Home
{
    public class ServiceItemChangedEventArgs : EventArgs
    {
        public object Item { get; set; }
    }

    public class ServiceItemValueChangedEventArgs : EventArgs
    {
        public object Item { get; set; }
        public object Value { get; set; }
    }
}
