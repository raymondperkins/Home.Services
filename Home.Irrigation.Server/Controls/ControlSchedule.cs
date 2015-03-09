using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ServiceStack;
using Raspberry.IO.GeneralPurpose;

using Home.Services;
namespace Home.Irrigation.Server
{
    public class ControlSchedule : Schedule
    {
        public string ControlUri { get; set; } 
        public string ControlId { get; set; }

        public ControlSchedule(string ControlId)
        {
            this.ControlUri = string.Format("http://localhost:{0}/", App.GetDeviceHttpPort());
            this.ControlId = ControlId;
        }

        public ControlSchedule(string ControlUri, string ControlId)
        {
            this.ControlUri = ControlUri;
            this.ControlId = ControlId;
        }

        public override void OnActivated()
        {
            App.Debug("ControlSchedule {0} becoming active", ControlId);
            using (var client = new JsonServiceClient(ControlUri) { UserName = "", Password = "", AlwaysSendBasicAuthHeader = true })
            {
                var response = client.Put(new PutControlSet() { Id = ControlId, Value = true });
            }
        }

        public override void OnDeactivated()
        {
            App.Debug("ControlSchedule {0} becoming inactive", ControlId);
            using (var client = new JsonServiceClient(ControlUri) { UserName = "", Password = "", AlwaysSendBasicAuthHeader = true })
            {
                var response = client.Put(new PutControlSet() { Id = ControlId, Value = false });
            }
        }
    }
}
