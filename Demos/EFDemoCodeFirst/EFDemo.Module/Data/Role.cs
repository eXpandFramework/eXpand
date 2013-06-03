using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Data.Objects.DataClasses;
using System.ComponentModel.DataAnnotations;

using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;

namespace EFDemo.Module.Data {
	public class Role : ISecurityRole, IOperationPermissionProvider {
		public Role() {
			Users = new List<User>();
			TypePermissions = new List<TypePermissionObject>();
			ChildRoles = new List<Role>();
			ParentRoles = new List<Role>();
		}
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		public String Name { get; set; }
		public Boolean IsAdministrative { get; set; }
		public Boolean CanEditModel { get; set; }
		public virtual IList<User> Users { get; set; }
		[Aggregated]
		public virtual IList<TypePermissionObject> TypePermissions { get; set; }
		public virtual IList<Role> ChildRoles { get; set; }
		public virtual IList<Role> ParentRoles { get; set; }
		
		// ISecurityRole
		String ISecurityRole.Name {
			get { return Name; }
		}
		// IOperationPermissionProvider
		IEnumerable<IOperationPermissionProvider> IOperationPermissionProvider.GetChildren() {
			List<IOperationPermissionProvider> result = new List<IOperationPermissionProvider>();
			result.AddRange(new EnumerableConverter<IOperationPermissionProvider, Role>(ChildRoles));
			return result;
		}
		IEnumerable<IOperationPermission> IOperationPermissionProvider.GetPermissions() {
			List<IOperationPermission> result = new List<IOperationPermission>();
			foreach(TypePermissionObject persistentPermission in TypePermissions) {
				result.AddRange(persistentPermission.GetPermissions());
			}
			if(IsAdministrative) {
				result.Add(new IsAdministratorPermission());
			}
			if(CanEditModel) {
				result.Add(new ModelOperationPermission());
			}
			return result;
		}
	}
}
