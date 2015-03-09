using System;
using System.Collections.Generic;

using ServiceStack;
using ServiceStack.Text;

namespace Home.Control.ServiceModel
{
    [Route("/networks/info", "GET")]
    public class Networks
    {
        
    }

    public class NetworksResponse
    {
        public bool HasCellular { get; set; }
        public bool HasWifi { get; set; }
        public CellularInfo CelluarInfo { get; set; }
        public WifiInfo WifiInfo { get; set; }
        public List<NetworkInfo> Records { get; set; }
    }

    public class NetworkInfo
    {
        public string Status { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Speed { get; set; }
        public string Type { get; set; }
    }

    public class CellularInfo
    {
        public string Status { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Speed { get; set; }
        public string Signal { get; set; }
        public string Type { get; set; }
    }

    public class WifiInfo
    {
        public string Name { get; set; }
        public string ConfigText { get; set; }
        public string[] ScanResults { get; set; }
    }
}
