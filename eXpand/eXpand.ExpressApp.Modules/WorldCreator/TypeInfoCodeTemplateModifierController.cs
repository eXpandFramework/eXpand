using DevExpress.ExpressApp;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Utils.Helpers;
using System.Linq;

namespace eXpand.ExpressApp.WorldCreator
{
    public class TypeInfoCodeTemplateModifierController : ViewController
    {
        public TypeInfoCodeTemplateModifierController()
        {
            TargetObjectType = typeof (IPersistentTypeInfo);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            ObjectSpace.ObjectChanged+=ObjectSpaceOnObjectChanged;
        }

        void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs objectChangedEventArgs)
        {
            if (objectChangedEventArgs.Object is IPersistentTypeInfo && objectChangedEventArgs.NewValue != null &&
                ((IPersistentTypeInfo) objectChangedEventArgs.Object).GetPropertyName(x => x.CodeTemplate) ==
                objectChangedEventArgs.PropertyName) {
                var persistentTypeInfo = ((IPersistentTypeInfo) objectChangedEventArgs.Object);
                var module =(WorldCreatorModule) Application.Modules.FindModule(typeof (WorldCreatorModule));
                persistentTypeInfo.TemplateInfo =(ITemplateInfo) ObjectSpace.CreateObject(module.TypesInfo.TemplateInfoType);
                var type = persistentTypeInfo.TemplateInfo.GetType();
                foreach (var info in typeof(ITemplateInfo).GetProperties().Select(propertyInfo =>new {
                                                                                                         Value=propertyInfo.GetValue(objectChangedEventArgs.NewValue,null),
                                                                                                         TypeInfoPropertyInfo=type.GetProperty(propertyInfo.Name)
                                                                                                     })) {
                    info.TypeInfoPropertyInfo.SetValue(persistentTypeInfo.TemplateInfo,info.Value,null);
                }
            }
            }
        }

    }

