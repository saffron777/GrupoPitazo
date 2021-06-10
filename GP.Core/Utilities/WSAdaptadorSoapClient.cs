using System;
using System.Collections.Generic;
using System.Text;

namespace WSA
{
    public partial class WSAdaptadorSoapClient
    {
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials)
        {
            serviceEndpoint.Address =
                new System.ServiceModel.EndpointAddress(new System.Uri(GP.Core.Utilities.Utils.settings.urlWSA),
                new System.ServiceModel.DnsEndpointIdentity(""));
        }
    }
}
