using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using Funq;

using ServiceStack;
using ServiceStack.Logging;
using ServiceStack.Razor;
using ServiceStack.Text;

using Home.Services;

namespace Home.Irrigation.Server
{
    public class AppHost : AppHostHttpListenerBase
    {
        public AppHost()
            : base("Home services",
                typeof(AppHost).Assembly,
                typeof(ScheduleService).Assembly,
                typeof(ControlsService).Assembly,
                typeof(SensorsService).Assembly) { }

        public override void Configure(Container container)
        {
            LogManager.LogFactory = new ConsoleLogFactory();

            Plugins.Add(new RazorFormat()
            {
                LoadFromAssemblies = { typeof(AppHost).Assembly }
            });
            ServiceStack.Text.JsConfig.DateHandler = ServiceStack.Text.DateHandler.ISO8601;


            this.CustomErrorHttpHandlers[HttpStatusCode.Unauthorized] = new RazorHandler("/login");
        }
    }

}
