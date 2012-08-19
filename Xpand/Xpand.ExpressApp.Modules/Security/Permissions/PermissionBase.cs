using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using Xpand.Utils.Helpers;
using Xpand.Xpo;

namespace Xpand.ExpressApp.Security.Permissions {
    public abstract class PermissionBase : DevExpress.ExpressApp.Security.PermissionBase {
        IEnumerable<PropertyInfo> _propertyInfos;

        protected PermissionBase() {
            EnumerateProperties();
        }

        void EnumerateProperties() {
            _propertyInfos = GetType().GetProperties().Where(info => info.GetSetMethod() != null && info.GetCustomAttributes(typeof(NonPersistentAttribute), true).Count() == 0);
        }


        public override SecurityElement ToXml() {
            return AllToXml();
        }
        public override void FromXml(SecurityElement e) {
            base.FromXml(e);
            AllFromXml(e);
        }
        public override string ToString() {
            return _propertyInfos.Aggregate<PropertyInfo, string>(null, (current, propertyInfo) => current + (propertyInfo.GetValue(this, null) + ", ")).TrimEnd(", ".ToCharArray());
        }

        protected void AllFromXml(SecurityElement e) {
            foreach (var propertyInfo in _propertyInfos)
                propertyInfo.SetValue(this, ChangeType(propertyInfo, e), null);
        }

        private object ChangeType(PropertyInfo propertyInfo, SecurityElement e) {
            var attribute = e.Attributes[propertyInfo.Name];
            if (attribute == null)
                return null;

            var typePropertyEditorIsUsed = propertyInfo.PropertyType == typeof(Type);
            if (!typePropertyEditorIsUsed)
                return XpandReflectionHelper.ChangeType(attribute.ToString().XMLDecode(), propertyInfo.PropertyType);

            return string.IsNullOrEmpty((attribute + "")) ? null : XafTypesInfo.Instance.FindTypeInfo(attribute.ToString()).Type;
        }


        protected SecurityElement AllToXml() {
            SecurityElement result = base.ToXml();
            foreach (var propertyInfo in _propertyInfos)
                result.AddAttribute(propertyInfo.Name, (propertyInfo.GetValue(this, null) + "").XMLEncode());
            return result;
        }
    }
}