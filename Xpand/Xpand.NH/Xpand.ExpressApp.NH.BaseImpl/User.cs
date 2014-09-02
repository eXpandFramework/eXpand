using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Text;

namespace Xpand.ExpressApp.NH.BaseImpl
{
    [DataContract]
    public class User : ISecurityUser, ISecurityUserWithRoles, IAuthenticationStandardUser, IOperationPermissionProvider
    {

        public User()
        {
            IsActive = true;
        }

        [DataMember]
        public Guid Id
        {
            get;
            set;
        }

        [DataMember]
        public bool IsActive
        {
            get;
            set;
        }

        [DataMember]
        public string UserName
        {
            get;
            set;
        }


        private List<Role> roles;
        [DataMember]
        public IList<Role> Roles
        {
            get
            {
                if (roles == null)
                    roles = new List<Role>();
                return roles;
            }
        }

        [DataMember]
        public bool ChangePasswordOnFirstLogon
        {
            get;
            set;
        }

        [DataMember]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        public string StoredPassword
        {
            get;
            set;
        }

        public bool ComparePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) && string.IsNullOrWhiteSpace(StoredPassword)) return true;

            return new PasswordCryptographer().AreEqual(StoredPassword, password);
        }

        public static string GeneratePassword(string password)
        {
            return new PasswordCryptographer().GenerateSaltedPassword(password);
        }
        public void SetPassword(string password)
        {
            StoredPassword = GeneratePassword(password);
        }



        public IEnumerable<IOperationPermissionProvider> GetChildren()
        {
            //TODO: Implement GetChildren
            yield break;
        }

        public IEnumerable<IOperationPermission> GetPermissions()
        {
            return roles.SelectMany(r => r.GetPermissions()).ToList().AsReadOnly();
        }



        protected IList<ISecurityRole> RolesAsReadOnly
        {
            get
            {
                return roles.Cast<ISecurityRole>().ToList().AsReadOnly();
            }
        }

        IList<ISecurityRole> ISecurityUserWithRoles.Roles
        {
            get { return RolesAsReadOnly; }
        }
    }
}