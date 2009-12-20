using DevExpress.ExpressApp;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.WorldCreator.Observers {
    public class CodeTemplateInfoObserver:ObjectObserver<ICodeTemplateInfo> {
        public CodeTemplateInfoObserver(ObjectSpace objectSpace) : base(objectSpace) {
        }

        protected override void OnChanged(ObjectChangedEventArgs<ICodeTemplateInfo> objectChangedEventArgs)
        {
            base.OnChanged(objectChangedEventArgs);
            if (objectChangedEventArgs.NewValue != null && objectChangedEventArgs.Object.GetPropertyName(x => x.CodeTemplate) == objectChangedEventArgs.PropertyName){
                objectChangedEventArgs.Object.CloneProperties();
            }

        }

    }
}