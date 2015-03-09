using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ServiceStack;
using ServiceStack.Text;
using Home;

namespace Home.Services
{
    public class SensorsService : Service, IHomeServiceController
    {
        public object Get(SensorsModel request)
        {
            Sensors response = SensorHomeExtensions.InstanceSensors;
            
            return response;
        }

        public object Get(SensorModel request)
        {
            // find the sensor that matches this Id
            ISensor response = SensorHomeExtensions.InstanceSensors.FirstOrDefault(q => q.Id == request.Id);
            if (response == null)
            {
                throw HttpError.NotFound(string.Format("{0}", request.Id));
            }

            return response;
        }

        public object Put(SensorModel request)
        {
            // find the sensor that matches this Id
            ISensor response = SensorHomeExtensions.InstanceSensors.FirstOrDefault(q => q.Id == request.Id);
            if (response == null)
            {
                throw HttpError.NotFound(string.Format("{0}", request.Id));
            }
            response.Name = request.Name;
            // notify Home instance that a change has occured, this is so it can do things like save the instances
            App.Instance.OnServiceInstanceItemChanged(response);
            return response;
        }
    }
}
