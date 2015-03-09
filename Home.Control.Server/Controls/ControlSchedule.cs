using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ServiceStack;
using Raspberry.IO.GeneralPurpose;

using Home.Services;
namespace Home.Control.Server
{
    public class ControlSchedule : Schedule
    {
        string controlUri;
        string controlId;

        public ControlSchedule(string ControlId)
        {
            this.controlUri = string.Format("http://localhost:{0}/", App.GetDeviceHttpPort());
            this.controlId = ControlId;
        }

        public ControlSchedule(string ControlUri, string ControlId)
        {
            this.controlUri = ControlUri;
            this.controlId = ControlId;
        }

        public override void OnActivated()
        {
            using (var client = new JsonServiceClient(controlUri) { UserName = "", Password = "", AlwaysSendBasicAuthHeader = true })
            {
                var response = client.Put(new PutControlSet() { Id = controlId, Value = true });
            }
        }

        public override void OnDeactivated()
        {
            using (var client = new JsonServiceClient(controlUri) { UserName = "", Password = "", AlwaysSendBasicAuthHeader = true })
            {
                var response = client.Put(new PutControlSet() { Id = controlId, Value = false });
            }
        }
    }
}
