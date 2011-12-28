using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Validation;
using Xpand.ExpressApp.MasterDetail.Logic;
using Xpand.ExpressApp.Win.ListEditors;

namespace Xpand.ExpressApp.MasterDetail.Win {
    public class MasterDetailValidationController : ViewController<ListView> {
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (IsMasterDetail) {
                var validationController = this.Frame.GetController<PersistenceValidationController>();
                if (validationController != null) {
                    validationController.NeedToValidateObject += ValidationController_NeedToValidateObject;
                }
            }
        }

        protected override void OnDeactivated() {
            if (GridListEditor != null && GridListEditor.GridView != null && GridListEditor.GridView.MasterFrame != null) {
                var validationController = this.Frame.GetController<PersistenceValidationController>();
                if (validationController != null) {
                    validationController.NeedToValidateObject -= ValidationController_NeedToValidateObject;
                }
            }

            base.OnDeactivated();
        }

        private void ValidationController_NeedToValidateObject(object sender, DevExpress.ExpressApp.Validation.NeedToValidateObjectEventArgs e) {
            e.NeedToValidate = (!ObjectSpace.IsDeletedObject(e.CurrentObject) && !ObjectSpace.IsObjectToDelete(e.CurrentObject));
        }

        XpandGridListEditor GridListEditor {
            get { return View != null ? View.Editor as XpandGridListEditor : null; }
        }

        public bool IsMasterDetail {
            get {
                return GridListEditor != null && Frame.GetController<MasterDetailRuleController>().MasterDetailRules.FirstOrDefault() != null;
            }
        }
    }
}
