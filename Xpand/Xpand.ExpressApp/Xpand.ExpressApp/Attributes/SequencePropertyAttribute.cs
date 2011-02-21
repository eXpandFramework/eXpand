using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Editors;

namespace Xpand.ExpressApp.Attributes {
    [AttributeUsage(AttributeTargets.Property)]
    public class SequencePropertyAttribute:Attribute,ICustomAttribute {
        string ICustomAttribute.Name {
            get { return "PropertyEditorType"; }
        }

        string ICustomAttribute.Value {
            get { return ReflectionHelper.FindTypeDescendants(XafTypesInfo.Instance.FindTypeInfo(typeof(IReleasedSequencePropertyEditor))).SingleOrDefault().FullName; }
        }
    }
}
