using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Persistent.Base.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Xpand.ExpressApp.NH.BaseImpl
{
    public class RoleBase<TTypePermission, TUser> : ISecurityRole, IOperationPermissionsProvider
        where TTypePermission : ITypePermission
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
            result.AddRange(TypePermissions.SelectMany(tp => tp.GetPermissions()));
            return result;
        }


        private IList<TUser> users;

        public IList<TUser> Users
        {
            get
            {
                if (users == null)
                    users = new List<TUser>();

                return users;
            }
        }


        private List<TTypePermission> typePermissions;

        public IList<TTypePermission> TypePermissions
        {
            get
            {
                if (typePermissions == null)
                    typePermissions = new List<TTypePermission>();

                return typePermissions;
            }
        }
    }
}
