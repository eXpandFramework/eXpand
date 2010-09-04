using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.Attributes;

namespace Xpand.ExpressApp.SystemModule {
    public class CustomAttibutesController : WindowController {
        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            foreach (XPClassInfo xpClassInfo in XafTypesInfo.XpoTypeInfoSource.XPDictionary.Classes) {
                foreach (XPMemberInfo member in xpClassInfo.Members) {
                    if (member.HasAttribute(typeof (NumericFormatAttribute))) {
                        member.AddAttribute(new CustomAttribute("EditMaskAttribute", "f0"));
                        member.AddAttribute(new CustomAttribute("DisplayFormatAttribute", "#"));
                        XafTypesInfo.Instance.RefreshInfo(xpClassInfo.ClassType);
                    }
                }
            }
        }
    }
}