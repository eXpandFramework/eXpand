using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.CustomAttributes;

namespace Xpand.Persistent.Base.General.Controllers {
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
                        if (!typeInfo.Implementors.Any()) {
                            if (Application != null)
                                throw new InvalidOperationException(typeInfo.FullName + " has no implementors");
                        }
                        else {
                            var single = typeInfo.Implementors.Single(info => !info.FindAttribute<DevExpress.ExpressApp.Editors.PropertyEditorAttribute>().IsDefaultEditor);
                            theValue = single.Type.FullName;
                        }
                        
                    }
                    memberInfo.AddAttribute(new ModelDefaultAttribute(s, theValue));
                }
            }
        }
    }
}