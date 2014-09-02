using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.ServiceModel;
using System.Text;
using Xpand.ExpressApp.NH.Core;

namespace Xpand.ExpressApp.NH
{
    public class RemotePersistenceManagerProxy : RealProxy
    {
        private readonly string url;

        public RemotePersistenceManagerProxy(string url)
            : base(typeof(IPersistenceManager))
        {
            if (url == null)
                throw new ArgumentNullException("url");
            this.url = url;
        }


        public override System.Runtime.Remoting.Messaging.IMessage Invoke(System.Runtime.Remoting.Messaging.IMessage msg)
        {
            IMethodCallMessage methodCallMessage = msg as IMethodCallMessage;
            MethodInfo methodInfo = (MethodInfo)methodCallMessage.MethodBase;
            using (var factory = CreateChannelFactory(url))
            {
                object result = methodInfo.Invoke(factory.CreateChannel(), methodCallMessage.InArgs);
                return new ReturnMessage(result, null, 0, methodCallMessage.LogicalCallContext, methodCallMessage);
            }
        }
        
        private static ChannelFactory<IPersistenceManager> CreateChannelFactory(string url)
        {

            Uri uri = new Uri(url);
            var channelFactory = new ChannelFactory<IPersistenceManager>(BindingFactory.CreateBasicBinding(uri), new EndpointAddress(uri));

            XpandDataContractResolver.AddToEndpoint(channelFactory.Endpoint);
            return channelFactory;
        }


    }
}
