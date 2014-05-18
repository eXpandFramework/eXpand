using System.ServiceModel;
using Xpand.ExpressApp.NH.Core;

namespace Xpand.ExpressApp.NH.Service
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(ServiceTypesHelper))]
    public interface IPersistenceManagerService : IPersistenceManager
    {

    }
}
