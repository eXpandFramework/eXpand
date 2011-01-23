using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.Attributes;
using Xpand.ExpressApp.Editors;

namespace Xpand.ExpressApp.SystemModule {
    public class CustomAttibutesController : WindowController {
        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            var memberInfos = typesInfo.PersistentTypes.SelectMany(info => info.OwnMembers);
            foreach (var memberInfo in memberInfos) {
                HandleNumericFormatAttribute(memberInfo);
                HandleSequencePropertyAttribute(memberInfo);
            }
        }

        void HandleSequencePropertyAttribute(IMemberInfo memberInfo) {
            var sequencePropertyAttribute = memberInfo.FindAttribute<SequencePropertyAttribute>();
            if (sequencePropertyAttribute != null) {
                var typeInfo = ReflectionHelper.FindTypeDescendants(XafTypesInfo.Instance.FindTypeInfo(typeof(IReleasedSequencePropertyEditor))).SingleOrDefault();
                if (typeInfo != null)
                    memberInfo.AddAttribute(new CustomAttribute("PropertyEditorType", typeInfo.FullName));
            }
        }

        void HandleNumericFormatAttribute(IMemberInfo memberInfo) {
            var numericFormatAttribute = memberInfo.FindAttribute<NumericFormatAttribute>();
            if (numericFormatAttribute != null) {
                memberInfo.AddAttribute(new CustomAttribute("EditMaskAttribute", "f0"));
                memberInfo.AddAttribute(new CustomAttribute("DisplayFormatAttribute", "#"));
            }
        }
    }
}