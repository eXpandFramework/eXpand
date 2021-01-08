using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.General.CustomAttributes {
    [AttributeUsage(AttributeTargets.Property)]
    public class SequencePropertyAttribute : Attribute, ICustomAttribute {
        public string Name => "PropertyEditorType";

        public string Value {
            get {
                ITypeInfo typeInfo = ReflectionHelper.FindTypeDescendants(XafTypesInfo.Instance.FindTypeInfo(typeof(IReleasedSequencePropertyEditor))).FirstOrDefault();
                return typeInfo != null ? typeInfo.FullName : "";
            }
        }

    }
    public interface IReleasedSequencePropertyEditor {
    }

}
