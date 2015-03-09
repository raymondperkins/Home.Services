using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Home.Services
{
    public interface ISensor
    {
        string Id { get; set; }
        string Name { get; set; }
        string Type { get; set; }
    }
}
