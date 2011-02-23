using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.SystemModule {
    public class CustomAttibutesController : WindowController {
        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            var memberInfos = typesInfo.PersistentTypes.SelectMany(info => info.OwnMembers);
            foreach (var memberInfo in memberInfos) {
                HandleCustomAttribute(memberInfo);
            }
        }

        void HandleCustomAttribute(IMemberInfo memberInfo) {
            var customAttributes = memberInfo.FindAttributes<Attribute>().OfType<ICustomAttribute>().ToList();
            foreach (var customAttribute in customAttributes) {
                for (int index = 0; index < customAttribute.Name.Split(';').Length; index++) {
                    string s = customAttribute.Name.Split(';')[index];
                    memberInfo.AddAttribute(new CustomAttribute(s, customAttribute.Value.Split(';')[index]));
                }
            }
        }
    }
}