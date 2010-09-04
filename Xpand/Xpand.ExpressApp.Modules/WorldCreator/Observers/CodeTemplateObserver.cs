using DevExpress.ExpressApp;
using Xpand.ExpressApp.Core;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.WorldCreator.Observers {
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