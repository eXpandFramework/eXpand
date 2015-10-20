using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Templates;

namespace Xpand.ExpressApp.ModelDifference.Win.Templates {
    public class ModelEditorDetailViewForm : DetailViewForm { 
        public override void SetSettings(IModelTemplate modelTemplate) {
            this.SetSettings(() => base.SetSettings(modelTemplate));
        }
    }
}
