using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace DCSecurityDemo.Module.Security {
    [DomainComponent]
    [ImageName("BO_Security_Permission_Type")]
    [XafDisplayName("Type Operation Permissions")]
    [XafDefaultProperty("Object")]
    public interface IDCTypePermissions : ITypePermissionOperations {
        [XafDisplayName("Read")]
        new Boolean AllowRead { get; set; }
        [XafDisplayName("Write")]
        new Boolean AllowWrite { get; set; }
        [XafDisplayName("Create")]
        new Boolean AllowCreate { get; set; }
        [XafDisplayName("Delete")]
        new Boolean AllowDelete { get; set; }
        [XafDisplayName("Navigate")]
        new Boolean AllowNavigate { get; set; }
        String Object { get; }
        [DevExpress.Xpo.ValueConverter(typeof(TypeToStringConverter))]
        [FieldSize(FieldSizeAttribute.Unlimited)]
        [VisibleInDetailView(false), VisibleInListView(false)]
        [RuleRequiredField("IDCTypePermissions_TargetType_RuleRequiredField", DefaultContexts.Save)]
        Type TargetType { get; set; }
        [Aggregated]
        IList<IDCMemberPermissions> MemberPermissions { get; }
        [Aggregated]
        IList<IDCObjectPermissions> ObjectPermissions { get; }

        IEnumerable<IOperationPermission> GetPermissions();
    }
    [DomainLogic(typeof(IDCTypePermissions))]
    public class IDCTypePermissionsLogic {
        public static String Get_Object(IDCTypePermissions typePermissions) {
            if(typePermissions.TargetType != null) {
                String classCaption = CaptionHelper.GetClassCaption(typePermissions.TargetType.FullName);
                return String.IsNullOrEmpty(classCaption) ? typePermissions.TargetType.Name : classCaption;
            }
            return String.Empty;
        }
        public static IEnumerable<IOperationPermission> GetPermissions(IDCTypePermissions typePermissions) {
            List<IOperationPermission> result = new List<IOperationPermission>();
            if(typePermissions.TargetType != null) {
                if(typePermissions.AllowRead) {
                    result.Add(new TypeOperationPermission(typePermissions.TargetType, SecurityOperations.Read));
                }
                if(typePermissions.AllowWrite) {
                    result.Add(new TypeOperationPermission(typePermissions.TargetType, SecurityOperations.Write));
                }
                if(typePermissions.AllowCreate) {
                    result.Add(new TypeOperationPermission(typePermissions.TargetType, SecurityOperations.Create));
                }
                if(typePermissions.AllowDelete) {
                    result.Add(new TypeOperationPermission(typePermissions.TargetType, SecurityOperations.Delete));
                }
                if(typePermissions.AllowNavigate) {
                    result.Add(new TypeOperationPermission(typePermissions.TargetType, SecurityOperations.Navigate));
                }
                foreach(IDCMemberPermissions memberPermissionOperations in typePermissions.MemberPermissions) {
                    result.AddRange(memberPermissionOperations.GetPermissions());
                }
                foreach(IDCObjectPermissions objectPermissionOperations in typePermissions.ObjectPermissions) {
                    result.AddRange(objectPermissionOperations.GetPermissions());
                }
            }
            return result;
        }
    }
}
