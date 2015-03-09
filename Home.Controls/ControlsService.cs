using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using ServiceStack;
using ServiceStack.Text;
using Home;

namespace Home.Services
{
    public class ControlsService : Service, IHomeServiceController
    {
        public object Get(GetControls request)
        {
            List<ControlModel> response = new List<ControlModel>();

            foreach (var item in ControlHomeExtensions.InstanceControls)
            {
                response.Add(ToModel(item));
            }

            return response;
        }
        /*
        public object Get(GetControl request)
        {
            // find the sensor that matches this Id
            IControl control = ControlHomeExtensions.InstanceControls.FirstOrDefault(q => q.Id == request.Id);
            if (control == null)
            {
                throw HttpError.NotFound(string.Format("{0}", request.Id));
            }

            return ToModel(control);
        }
        */
        public object Get(GetScheduleTemplate request)
        {
            // find the sensor that matches this Id
            IControl control = ControlHomeExtensions.InstanceControls.FirstOrDefault(q => q.Id == request.Id);
            if (control == null)
            {
                throw HttpError.NotFound(string.Format("{0}", request.Id));
            }

            // find the template file for this item.
            string name = control.GetType().Name;
            string basedir = Home.App.AssemblyDirectory;//AssemblyUtils.GetAssemblyBinPath(System.Reflection.Assembly.GetExecutingAssembly());
            //templatedir = Path.Combine(templatedir, "Views", "templates");
            string filePath = Path.Combine(basedir, "Views", "templates", string.Format("{0}.html", name));
            if (File.Exists(filePath))
            {
                // return file
                return File.OpenRead(filePath);
            }
            else
            {
                // return default
                return File.OpenRead(Path.Combine(basedir, "Views", "templates", "controlitem.html"));
            }
        }

        public object Put(PutControl request)
        {
            // find the sensor that matches this Id
            IControl control = ControlHomeExtensions.InstanceControls.FirstOrDefault(q => q.Id == request.Id);
            if (control == null)
            {
                throw HttpError.NotFound(string.Format("{0}", request.Id));
            }
            control.Name = request.Name;
            // process value for model
            if (request.Config != null)
            {
                object controlConfig = control.GetConfig();
                object newConfig = request.Config;
                if (newConfig.GetType() != controlConfig.GetType())
                {
                    var conv = System.ComponentModel.TypeDescriptor.GetConverter(controlConfig.GetType());
                    newConfig = conv.ConvertFrom(request.Value);
                }

                control.SetConfig(newConfig);
                // notify Home instance that a change has occured in the value
                App.Instance.OnServiceInstanceItemValueChanged(control);
            }

            // notify Home instance that a change has occured, this is so it can do things like save the instances
            App.Instance.OnServiceInstanceItemChanged(control);
            return ToModel(control);
        }

        public object Put(PutControlSet request)
        {
            // find the sensor that matches this Id
            IControl control = ControlHomeExtensions.InstanceControls.FirstOrDefault(q => q.Id == request.Id);
            if (control == null)
            {
                throw HttpError.NotFound(string.Format("{0}", request.Id));
            }
            // process value for model
            if (request.Value != null)
            {
                control.SetValue(request.Value.Value);
                // notify Home instance that a change has occured in the value
                App.Instance.OnServiceInstanceItemValueChanged(control);
            }

            return ToModel(control);
        }

        private ControlModel ToModel(IControl item)
        {
            var model = new ControlModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Type = item.Type,
                    Config = item.GetConfig(),
                    Value = item.GetValue()
                };
            return model;
        }
    }
}
