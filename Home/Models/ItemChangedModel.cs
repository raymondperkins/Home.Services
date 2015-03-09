using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Home.Models
{
    public class ItemChangedResponse
    {
        public bool success { get; set; }
        public string message { get; set; }

        public static ItemChangedResponse New(bool success, string message)
        {
            return new ItemChangedResponse()
            {
                success = success,
                message = message
            };
        }
    }
}
