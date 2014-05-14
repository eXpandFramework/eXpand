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
        public Guid Id { get; set; }

        [DataMember]
        public string Name
        {
            get;
            set;
        }

        [DataMember]
        public bool IsAdministrative { get; set; }

        [DataMember]
        public bool CanEditModel { get; set; }

        public IList<IOperationPermission> GetPermissions()
        {
            List<IOperationPermission> result = new List<IOperationPermission>();
            if (IsAdministrative)
            {
                result.Add(new IsAdministratorPermission());
            }
            if (CanEditModel)
            {
                result.Add(new ModelOperationPermission());
            }
            return result;
        }


        private IList<User> users;

        public IList<User> Users
        {
            get
            {
                if (users == null)
                    users = new List<User>();

                return users;
            }
        }
    }
}
