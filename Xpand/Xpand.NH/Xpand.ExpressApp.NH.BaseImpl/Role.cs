using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Xpand.ExpressApp.NH.BaseImpl
{
    public class Role : ISecurityRole, IOperationPermissionsProvider
    {

        [DataMember]
        public string Name
        {
            get;
            set;
        }

        public IList<IOperationPermission> GetPermissions()
        {
            throw new NotImplementedException();
        }
    }
}
