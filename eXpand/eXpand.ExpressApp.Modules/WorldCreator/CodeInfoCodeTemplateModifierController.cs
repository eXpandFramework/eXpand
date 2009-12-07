using DevExpress.ExpressApp;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Utils.Helpers;
using System.Linq;

namespace eXpand.ExpressApp.WorldCreator
{
    public class CodeInfoCodeTemplateModifierController : ViewController
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            ObjectSpace.ObjectChanged+=ObjectSpaceOnObjectChanged;
        }

        void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs objectChangedEventArgs)
        {
            if (View!= null&& objectChangedEventArgs.Object is ICodeTemplateInfo && objectChangedEventArgs.NewValue != null &&
                ((ICodeTemplateInfo)objectChangedEventArgs.Object).GetPropertyName(x => x.CodeTemplate) ==
                objectChangedEventArgs.PropertyName) {
                var persistentTypeInfo = ((ICodeTemplateInfo)objectChangedEventArgs.Object);
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

