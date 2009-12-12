using DevExpress.ExpressApp;
using eXpand.ExpressApp.Core;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.WorldCreator.Observers {
    public class CodeTemplateObserver:ObjectObserver<ICodeTemplate> {
        public CodeTemplateObserver(ObjectSpace objectSpace) : base(objectSpace) {
        }
        protected override void OnChanged(ObjectChangedEventArgs<ICodeTemplate> e)
        {
            base.OnChanged(e);
            if (e.Object.GetPropertyName(x => x.TemplateType) ==e.PropertyName)
                e.Object.SetDefaults();  

        }
    }
}