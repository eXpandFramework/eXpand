using System.Collections.Generic;
using System.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace eXpand.Persistent.TaxonomyImpl{
    [ImageName("BO_User"), System.ComponentModel.DisplayName("User")]
    public class User : Person, IUserWithRoles, IAuthenticationActiveDirectoryUser, IAuthenticationStandardUser{
        private readonly UserImpl user;
        private readonly List<IPermission> permissions;

        public User(Session session) : base(session){
            permissions = new List<IPermission>();
            user = new UserImpl(this);
        }

        public void ReloadPermissions(){
            Roles.Reload();
            foreach (Role role in Roles){
                role.PersistentPermissions.Reload();
            }
        }

        public bool ComparePassword(string password){
            return user.ComparePassword(password);
        }

        public void SetPassword(string password){
            user.SetPassword(password);
        }

#if MediumTrust
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Persistent]
		public string StoredPassword {
			get { return user.StoredPassword; }
			set {
				user.StoredPassword = value;
				OnChanged("StoredPassword");
			}
		}
#else
        [Persistent]
        private string StoredPassword{
            get { return user.StoredPassword; }
            set{
                user.StoredPassword = value;
                OnChanged("StoredPassword");
            }
        }
#endif

        [Association("User-Role")]
        public XPCollection<Role> Roles{
            get { return GetCollection<Role>("Roles"); }
        }

        IList<IRole> IUserWithRoles.Roles{
            get { return new ListConverter<IRole, Role>(Roles); }
        }

        [RuleRequiredField("User Name required", "Save", "The user name must not be empty")]
        [RuleUniqueValue("User Name is unique", "Save", "The login with the entered UserName was already registered within the system")]
        public string UserName{
            get { return user.UserName; }
            set{
                user.UserName = value;
                OnChanged("UserName");
            }
        }

        public bool ChangePasswordOnFirstLogon{
            get { return user.ChangePasswordAfterLogon; }
            set{
                user.ChangePasswordAfterLogon = value;
                OnChanged("ChangePasswordOnFirstLogon");
            }
        }

        public bool IsActive{
            get { return user.IsActive; }
            set{
                user.IsActive = value;
                OnChanged("IsActive");
            }
        }

        public IList<IPermission> Permissions{
            get{
                permissions.Clear();
                foreach (Role role in Roles){
                    permissions.AddRange(role.Permissions);
                }
                return permissions.AsReadOnly();
            }
        }
    }
}