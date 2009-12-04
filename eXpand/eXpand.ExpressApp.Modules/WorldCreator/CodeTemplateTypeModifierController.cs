using DevExpress.ExpressApp;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.WorldCreator
{
    public class CodeTemplateTypeModifierController : ViewController
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            ObjectSpace.ObjectChanged+=ObjectSpaceOnObjectChanged;
        }

        void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs objectChangedEventArgs) {
            if (objectChangedEventArgs.Object is ICodeTemplate &&
                ((ICodeTemplate) objectChangedEventArgs.Object).GetPropertyName(x => x.TemplateType) ==
                objectChangedEventArgs.PropertyName)
                ((ICodeTemplate) objectChangedEventArgs.Object).SetDefaults();  
        }
    }
}
