using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Funq;

using ServiceStack;
using Home.Services;

namespace Home.Tests
{
    public class TestHost : AppHostHttpListenerBase
    {
        public TestHost()
            : base("Test Host no razor",
                typeof(SensorsService).Assembly,
                typeof(ControlsService).Assembly,
                typeof(ScheduleService).Assembly) 
        { }

        public override void Configure(Container container)
        {
            
            ServiceStack.Text.JsConfig.DateHandler = ServiceStack.Text.DateHandler.ISO8601;


        }


        public static TestHost CreateAndStartHost(string uri)
        {
            //Streats.Licensing.RegisterServiceStackLicense();

            var appHost = new TestHost();

            appHost.Init();
            appHost.Start(uri);

            return appHost;
        }

        public static JsonServiceClient CreateAndAuthClient(string uri, string user, string pass)
        {
            var client = new JsonServiceClient(uri);
            client.UserName = user;
            client.Password = pass;
            client.OnAuthenticationRequired = (System.Net.WebRequest arg) =>
            {

            };
            /*
            var authResponse = client.Post<AuthenticateResponse>("/auth",
                new ServiceStack.Authenticate
                {
                    UserName = user,
                    Password = pass,
                    RememberMe = true,  //important tell client to retain permanent cookies
                });
             */
            return client;
        }
    }
}
