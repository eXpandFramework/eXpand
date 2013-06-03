using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;

namespace EFDemo.Module.Data {
	public class User : ISecurityUser, IAuthenticationActiveDirectoryUser, IAuthenticationStandardUser, IOperationPermissionProvider, ISecurityUserWithRoles {
		public User() {
			IsActive = true;
			Roles = new List<Role>();
		}
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		public String UserName { get; set; }
		public Boolean IsActive { get; set; }
		public Boolean ChangePasswordOnFirstLogon { get; set; }
		[Browsable(false), SecurityBrowsable]
		public String StoredPassword { get; set; }
		public virtual IList<Role> Roles { get; set; }

		// ISecurityUser
		Boolean ISecurityUser.IsActive {
			get { return IsActive; }
		}
		String ISecurityUser.UserName {
			get { return UserName; }
		}

		// IAuthenticationActiveDirectoryUser
		String IAuthenticationActiveDirectoryUser.UserName {
			get { return UserName; }
			set { UserName = value; }
		}

		// IAuthenticationStandardUser
		Boolean IAuthenticationStandardUser.ComparePassword(String password) {
			PasswordCryptographer passwordCryptographer = new PasswordCryptographer();
			return passwordCryptographer.AreEqual(StoredPassword, password);
		}
		Boolean IAuthenticationStandardUser.ChangePasswordOnFirstLogon {
			get { return ChangePasswordOnFirstLogon; }
			set { ChangePasswordOnFirstLogon = value; }
		}
		String IAuthenticationStandardUser.UserName {
			get { return UserName; }
		}

		// IOperationPermissionProvider
		IEnumerable<IOperationPermissionProvider> IOperationPermissionProvider.GetChildren() {
			return new EnumerableConverter<IOperationPermissionProvider, Role>(Roles);
		}
		IEnumerable<IOperationPermission> IOperationPermissionProvider.GetPermissions() {
			return new IOperationPermission[0];
		}

		// ISecurityUserWithRoles
		IList<ISecurityRole> ISecurityUserWithRoles.Roles {
			get {
				IList<ISecurityRole> result = new List<ISecurityRole>();
				foreach(Role role in Roles) {
					result.Add(role);
				}
				return new ReadOnlyCollection<ISecurityRole>(result);
			}
		}

		public void SetPassword(String password) {
			PasswordCryptographer passwordCryptographer = new PasswordCryptographer();
			StoredPassword = passwordCryptographer.GenerateSaltedPassword(password);
		}
	}
}
