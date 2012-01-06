using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.Security.Permissions {
    public abstract class XpandPermissionData:PermissionData {
        IEnumerable<PropertyInfo> _propertyInfos;


        protected XpandPermissionData(Session session) : base(session) {
            EnumerateProperties();
        }
        protected override string GetPermissionInfoCaption() {
            return _propertyInfos.Aggregate<PropertyInfo, string>(null, (current, propertyInfo) => current + (propertyInfo.GetValue(this, null) + ", ")).TrimEnd(", ".ToCharArray());
        }
        void EnumerateProperties() {
            _propertyInfos = GetType().GetProperties().Where(info => info.GetSetMethod() != null && info.GetCustomAttributes(typeof(NonPersistentAttribute), true).Count() == 0);
        }

    }
}