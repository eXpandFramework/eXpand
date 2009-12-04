using DevExpress.ExpressApp;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.WorldCreator {
    public class TypeInfoTemplateModifierController:ViewController {
        protected override void OnActivated()
        {
            base.OnActivated();
            ObjectSpace.ObjectChanged+=ObjectSpaceOnObjectChanged;
        }

        void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs objectChangedEventArgs) {
            if (objectChangedEventArgs.Object is IPersistentTypeInfo && ((IPersistentTypeInfo)objectChangedEventArgs.Object).GetPropertyName(x => x.CodeTemplate) == objectChangedEventArgs.PropertyName) {
                var persistentTypeInfo = ((IPersistentTypeInfo)objectChangedEventArgs.Object);
                persistentTypeInfo.GeneratedCode=persistentTypeInfo.CodeTemplate.TemplateCode;
            }
        }
    }
}