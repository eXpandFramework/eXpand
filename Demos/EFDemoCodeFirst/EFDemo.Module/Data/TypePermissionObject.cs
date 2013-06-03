using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Security;

namespace EFDemo.Module.Data {
	public class TypePermissionObject {
		private Type targetType;
		public IEnumerable<IOperationPermission> GetPermissions() {
			List<IOperationPermission> result = new List<IOperationPermission>();
			if(TargetType != null) {
				if(AllowRead) {
					result.Add(new TypeOperationPermission(TargetType, SecurityOperations.Read));
				}
				if(AllowWrite) {
					result.Add(new TypeOperationPermission(TargetType, SecurityOperations.Write));
				}
				if(AllowCreate) {
					result.Add(new TypeOperationPermission(TargetType, SecurityOperations.Create));
				}
				if(AllowDelete) {
					result.Add(new TypeOperationPermission(TargetType, SecurityOperations.Delete));
				}
				if(AllowNavigate) {
					result.Add(new TypeOperationPermission(TargetType, SecurityOperations.Navigate));
				}
			}
			return result;
		}
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		public Boolean AllowRead { get; set; }
		public Boolean AllowWrite { get; set; }
		public Boolean AllowCreate { get; set; }
		public Boolean AllowDelete { get; set; }
		public Boolean AllowNavigate { get; set; }
		[Browsable(false)]
		public String TargetTypeFullName { get; protected set; }
		public Role Role { get; set; }

		[NotMapped]
		public Type TargetType {
			get {
				if((targetType == null) && !String.IsNullOrWhiteSpace(TargetTypeFullName)) {
					targetType = ReflectionHelper.FindType(TargetTypeFullName);
				}
				return targetType;
			}
			set {
				targetType = value;
				if(targetType != null) {
					TargetTypeFullName = targetType.FullName;
				}
				else {
					TargetTypeFullName = "";
				}
			}
		}
	}
}
