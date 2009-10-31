using System.Security;
using System.Collections.ObjectModel;
using System.ComponentModel;
using DevExpress.Xpo;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;

namespace eXpand.Persistent.TaxonomyImpl{
    [DefaultProperty("Name")]
    public class RoleBase : BaseObject {
        private RoleImpl role = new RoleImpl();
        public RoleBase(Session session) : base(session) { }
        public PersistentPermission AddPermission(IPermission permission) {
            PersistentPermission result = new PersistentPermission(Session, permission);
            PersistentPermissions.Add(result);
            return result;
        }
        [MemberDesignTimeVisibility(false), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorCollectionReturnsNewValueOnEachAccess))]
        public ReadOnlyCollection<IPermission> Permissions {
            get { return role.GetPermissions(new ListConverter<IPersistentPermission, PersistentPermission>(PersistentPermissions)); }
        }
        public string Name {
            get { return role.Name; }
            set {
                role.Name = value;
                OnChanged("Name");
            }
        }
        [Aggregated, Association("Role-PersistentPermissions")]
        public XPCollection<PersistentPermission> PersistentPermissions {
            get { return GetCollection<PersistentPermission>("PersistentPermissions"); }
        }
    }

    public class Role : RoleBase, IRole, ICustomizableRole {
        public Role(Session session) : base(session) { }
        [Association("User-Role")]
        public XPCollection<User> Users {
            get { return GetCollection<User>("Users"); }
        }
        IList<IUser> IRole.Users {
            get {
                return new ListConverter<IUser, User>(Users);
            }
        }
        void ICustomizableRole.AddPermission(IPermission permission) {
            base.AddPermission(permission);
        }
    }
}