using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General.CustomAttributes;

namespace Xpand.ExpressApp.SystemModule {
    public class CustomAttibutesController : WindowController {
        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            var memberInfos = typesInfo.PersistentTypes.SelectMany(info => info.OwnMembers);
            foreach (var memberInfo in memberInfos) {
                HandleCustomAttribute(memberInfo, typesInfo);
            }
        }

        void HandleCustomAttribute(IMemberInfo memberInfo, ITypesInfo typesInfo) {
            var customAttributes = memberInfo.FindAttributes<Attribute>().OfType<ICustomAttribute>().ToList();
            foreach (var customAttribute in customAttributes) {
                for (int index = 0; index < customAttribute.Name.Split(';').Length; index++) {
                    string s = customAttribute.Name.Split(';')[index];
                    var theValue = customAttribute.Value.Split(';')[index];
                    var typeInfo = typesInfo.FindTypeInfo(theValue);
                    if (customAttribute is PropertyEditorAttribute && typeInfo.IsInterface) {
                        try {
                            theValue = typeInfo.Implementors.Single().FullName;
                        }
                        catch (InvalidOperationException) {
                            if (Application!=null)
                                throw new InvalidOperationException(typeInfo.FullName +" has no implementors");
                        }
                    }
                    memberInfo.AddAttribute(new CustomAttribute(s, theValue));
                }
            }
        }
    }
}