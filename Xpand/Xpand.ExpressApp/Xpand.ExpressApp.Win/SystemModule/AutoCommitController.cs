using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Actions;
using Xpand.ExpressApp.SystemModule;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class AutoCommitController: ExpressApp.SystemModule.AutoCommitController {
        private IEnumerable<ActionBase> _actionBases;
        protected override void OnActivated() {
            base.OnActivated();
            if (((IModelObjectViewAutoCommit)View.Model).AutoCommit) {
                _actionBases = Frame.Actions().Available();
                foreach (var actionBase in _actionBases) {
                    actionBase.Executing += ActionBaseOnExecuting;
                }
            }
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (((IModelObjectViewAutoCommit)View.Model).AutoCommit) {
                CommitChanges();
                foreach (var actionBase in _actionBases) {
                    actionBase.Executing -= ActionBaseOnExecuting;
                }
            }
        }

        private void ActionBaseOnExecuting(object sender, CancelEventArgs cancelEventArgs) {
            CommitChanges();
        }
    }
}
