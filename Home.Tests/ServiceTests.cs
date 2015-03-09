using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Data;
using ServiceStack.Logging;

using Mono.Upnp;

using Home;
using Home.Services;

namespace Home.Tests
{
    [TestClass]
    public class ServiceTests
    {
        readonly object mutex = new object();
        static TestHost Host
        {
            get
            {
                if (appHost == null)
                {
                    appHost = TestHost.CreateAndStartHost(AppHostUri);
                }
                return appHost;
            }
        }
        public static DateTime? TestScheduleTimeActivated { get; set; }
        public static DateTime? TestScheduleTimeDeactivated { get; set; }

        public class TestSchedule : Schedule
        {
            public override void OnActivated()
            {
                ServiceTests.TestScheduleTimeActivated = DateTime.Now;
                base.OnActivated();
            }

            public override void OnDeactivated()
            {
                ServiceTests.TestScheduleTimeDeactivated = DateTime.Now;
                base.OnDeactivated();
            }
        }

        public class TestControl : Control<bool>
        {
            bool controlValue = false;

            public override void SetControlValue(bool value)
            {
                controlValue = value;
            }

            public override bool GetControlValue()
            {
                return controlValue;
            }
        }

        public class TestComplexControlConfig
        {
            public bool Enabled { get; set; }
            public double Setpoint { get; set; }
        }

        public class TestComplexControl : Control<TestComplexControlConfig>
        {
            TestComplexControlConfig controlConfig = new TestComplexControlConfig
                {
                    Enabled = false,
                    Setpoint = 0
                };

            public override void SetControlValue(bool value)
            {
                controlConfig.Enabled = value;
            }

            public override bool GetControlValue()
            {
                return controlConfig.Enabled;
            }

            public override void SetControlConfig(TestComplexControlConfig value)
            {
                controlConfig.Enabled = value.Enabled;
                controlConfig.Setpoint = value.Setpoint;
            }

            public override TestComplexControlConfig GetControlConfig()
            {
                return controlConfig;
            }
        }

        private static string AppHostUri = "http://localhost:2337/";
        private static string AppHostUser = "Administrator";
        private static string AppHostPass = "admin";
        private static string CSVFolderPath = @"E:\Repositories\Streats.Products.Dataserver.Tests\CSVFiles";
        private static TestHost appHost;
        private static JsonServiceClient client;


        [ClassInitialize]
        public static void TestFixtureSetUp(TestContext testContext)
        {
            //Streats.Licensing.RegisterServiceStackLicense();

            appHost = TestHost.CreateAndStartHost(AppHostUri);
            client = TestHost.CreateAndAuthClient(AppHostUri, AppHostUser, AppHostPass);
        }

        [ClassCleanup]
        public static void TestFixtureTearDown()
        {
            appHost.Dispose();
        }

        [TestMethod]
        public void ServicesInitOk()
        {
            Assert.IsTrue(Host.HasStarted, "Host started");

            App.Instance.InitSchedular();
            App.Instance.InitControls();
            App.Instance.InitSensors();
            
            // check Upnp services started ok
            List<Mono.Upnp.Service> services = new List<Mono.Upnp.Service>();
            using (var client = new Client())
            {
                bool flag = false;
                
                client.ServiceAdded += (sender, args) =>
                {
                    services.Add(args.Service.GetService());
                };
                client.Browse(new ServiceType("schemas-upnp-org", "home-services-controls", new Version(1, 0)));
                client.Browse(new ServiceType("schemas-upnp-org", "home-services-sensors", new Version(1, 0)));
                client.Browse(new ServiceType("schemas-upnp-org", "home-services-schedular", new Version(1, 0)));

                App.StartInstance();

                Thread.Sleep(2000);
            }
            Assert.IsTrue(services.Count >= 3);
        }

        [TestMethod]
        public void TestSchedular()
        {
            TestScheduleTimeActivated = null;
            TestScheduleTimeDeactivated = null;

            var schedular = App.Instance.GetSchedular();

            DateTime timeStart = DateTime.Now.AddSeconds(1);
            TestSchedule testScheduleNoReoccur = new TestSchedule()
            {                
                ActiveSeconds = 3,
                ReoccurSeconds = 0,
                Id = "NoReoccur",
                Name = "NoReoccur",
                From = timeStart,
            };
            schedular.AddSchedule(testScheduleNoReoccur);

            TestSchedule testScheduleReoccur = new TestSchedule()
            {
                ActiveSeconds = 3,
                ReoccurSeconds = 3,
                Id = "Reoccur",
                Name = "Reoccur",
                From = timeStart,
            };
            schedular.AddSchedule(testScheduleReoccur);

            schedular.StartSchedular();
            Assert.IsFalse(testScheduleNoReoccur.IsActive);
            Assert.IsFalse(testScheduleReoccur.IsActive);

            Thread.Sleep(2000);

            Assert.IsTrue(testScheduleNoReoccur.IsActive);
            Assert.IsTrue(testScheduleReoccur.IsActive);

            Thread.Sleep(3000);

            Assert.IsFalse(testScheduleNoReoccur.IsActive);
            Assert.IsFalse(testScheduleReoccur.IsActive);

            Assert.IsTrue(testScheduleReoccur.From > timeStart);

            Assert.IsTrue(schedular.StopSchedular());
            Assert.IsFalse(schedular.IsRunning);
        }

        [TestMethod]
        public void TestControls()
        {
            TestScheduleTimeActivated = null;
            TestScheduleTimeDeactivated = null;

            var controls = App.Instance.GetControls();

            var control1 = new TestControl()
            {
                Name = "control1",                
            };

            controls.Add(control1);

            var matched1 = App.Instance.GetControlByName("control1");
            Assert.IsNotNull(matched1);
            Assert.AreEqual("control1", matched1.Name);
            matched1.SetValue(true);
            Assert.IsTrue(matched1 is TestControl);
            Assert.IsTrue((matched1 as TestControl).GetControlValue());
            matched1.SetValue(false);
            Assert.IsFalse((matched1 as TestControl).GetControlValue());
        }

        [TestMethod]
        public void TestControlsService()
        {
            var controls = App.Instance.GetControls();
            for (int i = controls.Count; i <= 5; i++)
            {
                controls.Add(new TestControl()
                {
                    Name = "control" + i.ToString(),
                });
            }
            // fetch via the web service
            var rescontrols = client.Get(new GetControls());

            Assert.AreEqual(controls.Count, rescontrols.Count);

            // toggle the test controls
            foreach (TestControl control in controls)
            {
                control.SetControlValue(true);
                var rescontrol = client.Get(new GetControl() { Id = control.Id });
                Assert.AreEqual("true", rescontrol.Value);
                client.Put(new PutControlSet() { Id = control.Id, Value = false });
                Assert.AreEqual(false, control.GetValue());
            }

            // test Complex control
            var complexcontroladded = new TestComplexControl()
            {
                Name = "complex1",
                Id = "complex1",
            };
            controls.Add(complexcontroladded);

            var complexcontrol = client.Get(new GetControl() { Id = "complex1" });
            Assert.IsNotNull(complexcontrol);
            Assert.IsInstanceOfType(complexcontrol.Config, typeof(TestComplexControlConfig));
            var config = new TestComplexControlConfig { Enabled = true, Setpoint = 100 };
            client.Put(new PutControl() { Id = complexcontrol.Id, Config = config });
            Assert.IsTrue(complexcontroladded.Value);
            Assert.AreEqual(100, complexcontroladded.Config.Setpoint);
            client.Put(new PutControlSet() { Id = complexcontrol.Id, Value = false });
            Assert.IsFalse(complexcontroladded.Value);
        }
    }
}
