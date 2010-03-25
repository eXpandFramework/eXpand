using System;
using System.Reflection;
using System.Security;
using DevExpress.ExpressApp;
using eXpand.Xpo;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.Security.Permissions
{
    public abstract class PermissionBase : DevExpress.ExpressApp.Security.PermissionBase
    {
        public override SecurityElement ToXml()
        {
            return AllToXml();
        }
        public override void FromXml(SecurityElement e)
        {
            base.FromXml(e);
            AllFromXml(e);
        }

        protected void AllFromXml(SecurityElement e)
        {
            foreach (var propertyInfo in GetType().GetProperties())
                propertyInfo.SetValue(this, ChangeType(propertyInfo, e), null);
        }

        private object ChangeType(PropertyInfo propertyInfo, SecurityElement e)
        {
            var typePropertyEditorIsUsed = propertyInfo.PropertyType==typeof (Type);
            if (!typePropertyEditorIsUsed)
                return ReflectorHelper.ChangeType(e.Attributes[propertyInfo.Name].ToString().XMLDecode(), propertyInfo.PropertyType);
            return string.IsNullOrEmpty((e.Attributes[propertyInfo.Name]+"")) ? null : XafTypesInfo.Instance.FindTypeInfo(e.Attributes[propertyInfo.Name].ToString()).Type;
        }

        
        protected SecurityElement AllToXml()
        {
            SecurityElement result = base.ToXml();
            foreach (var propertyInfo in GetType().GetProperties())
                result.AddAttribute(propertyInfo.Name, (propertyInfo.GetValue(this, null) + "").XMLEncode());
            return result;
        }
    }
}