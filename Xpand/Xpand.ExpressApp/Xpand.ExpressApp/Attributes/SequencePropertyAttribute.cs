using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Editors;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Attributes {
    [AttributeUsage(AttributeTargets.Property)]
    public class SequencePropertyAttribute:Attribute,ICustomAttribute {
        string ICustomAttribute.Name {
            get { return "PropertyEditorType"; }
        }

        string ICustomAttribute.Value {
            get {
                ITypeInfo typeInfo = ReflectionHelper.FindTypeDescendants(XafTypesInfo.Instance.FindTypeInfo(typeof(IReleasedSequencePropertyEditor))).SingleOrDefault();
                return typeInfo != null ? typeInfo.FullName : "";
            }
        }
    }
}
