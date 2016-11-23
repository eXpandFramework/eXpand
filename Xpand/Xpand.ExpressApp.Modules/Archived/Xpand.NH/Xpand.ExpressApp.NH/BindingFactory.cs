using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Xpand.ExpressApp.NH
{
    public static class BindingFactory
    {
        public static BasicHttpBinding CreateBasicBinding(Uri uri)
        {

            BasicHttpBinding binding = new BasicHttpBinding()
                {
                    CloseTimeout = TimeSpan.FromMinutes(1),
                    ReceiveTimeout = TimeSpan.FromMinutes(10),
                    SendTimeout = TimeSpan.FromMinutes(5),
                    AllowCookies = false,
                    BypassProxyOnLocal = false,
                    HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
                    MaxBufferSize = int.MaxValue,
                    MaxBufferPoolSize = 524288,
                    MaxReceivedMessageSize = int.MaxValue,
                    MessageEncoding = WSMessageEncoding.Text,
                    TextEncoding = Encoding.UTF8,
                    TransferMode = TransferMode.Buffered,
                    UseDefaultWebProxy = true
                };


            var quotas = binding.ReaderQuotas;
            quotas.MaxArrayLength =
                quotas.MaxBytesPerRead =
                quotas.MaxDepth =
                quotas.MaxNameTableCharCount =
                quotas.MaxStringContentLength = int.MaxValue;

            binding.Security.Mode = string.Equals(uri.Scheme, "https", StringComparison.OrdinalIgnoreCase) ? 
                BasicHttpSecurityMode.Transport : BasicHttpSecurityMode.TransportCredentialOnly;

            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
            return binding;
        }
    }
}
