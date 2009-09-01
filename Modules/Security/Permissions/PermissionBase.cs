using System;
using System.Linq;
using System.Reflection;
using System.Security;
using DevExpress.ExpressApp;
using eXpand.Xpo;

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
            foreach (var propertyInfo in GetType().GetProperties().Cast<PropertyInfo>())
                propertyInfo.SetValue(this, ChangeType(propertyInfo, e), null);
        }

        private object ChangeType(PropertyInfo propertyInfo, SecurityElement e)
        {
            var typePropertyEditorIsUsed = propertyInfo.PropertyType==typeof (Type);
            if (!typePropertyEditorIsUsed)
                return ReflectorHelper.ChangeType(XMLDecode(e.Attributes[propertyInfo.Name].ToString()), propertyInfo.PropertyType);
            return string.IsNullOrEmpty(e.Attributes[propertyInfo.Name].ToString())? null: XafTypesInfo.Instance.FindTypeInfo(e.Attributes[propertyInfo.Name].ToString()).Type;
        }

        public string XMLEncode(string Value)
        {
            return Value.Replace("&", "&amp;").Replace("'", "&apos;").Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;");
        }

        public string XMLDecode(string Value)
        {
            return Value.Replace("&amp;", "&").Replace("&apos;", "'").Replace("&quot;", "\"").Replace("&lt;", "<").Replace("&gt;", ">");
        }
        protected SecurityElement AllToXml()
        {
            SecurityElement result = base.ToXml();
            foreach (var propertyInfo in GetType().GetProperties().Cast<PropertyInfo>())
                result.AddAttribute(propertyInfo.Name, XMLEncode(propertyInfo.GetValue(this, null) + ""));
            return result;
        }
    }
}