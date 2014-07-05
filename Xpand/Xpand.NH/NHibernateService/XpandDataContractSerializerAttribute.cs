using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel.Description;
using Xpand.ExpressApp.NH;
using Xpand.ExpressApp.NH.Core;
using Xpand.ExpressApp.NH.DataLayer;

namespace Xpand.ExpressApp.NH.Service
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class XpandDataContractSerializerAttribute : Attribute, IServiceBehavior
    {

        private readonly bool enabled;

        public XpandDataContractSerializerAttribute(bool enabled)
        {
            this.enabled = enabled;
        }

        public bool Enabled { get { return enabled; } }

        public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            AttachBehavior(serviceDescription);
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            AttachBehavior(serviceDescription);
        }

        private void AttachBehavior(ServiceDescription serviceDescription)
        {
            if (enabled)
            {
                foreach (var endPoint in serviceDescription.Endpoints)
                    XpandDataContractResolver.AddToEndpoint(endPoint);
            }
        }

        public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
        }
    }
}
