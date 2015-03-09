using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Upnp.Control;

namespace Home
{
    public class UpnpServiceController : ServiceController
    {
        public UpnpServiceController()
            : base((IEnumerable<ServiceAction>)null, null)
        {
        }
    }
}
