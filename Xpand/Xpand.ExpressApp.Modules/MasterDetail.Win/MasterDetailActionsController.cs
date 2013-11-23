using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using Xpand.ExpressApp.MasterDetail.Model;
using System.Linq;

namespace Xpand.ExpressApp.MasterDetail.Win {
    public class MasterDetailActionsController : ExpressApp.Win.ListEditors.GridListEditors.GridView.MasterDetail.MasterDetailActionsController {
        protected override IEnumerable<ActionBase> GetActions(Frame frame) {
            var actions = base.GetActions(frame);
            var excludedFromSynchronization = ((IModelApplicationMasterDetail) Application.Model).MasterDetail.ActionsExcludedFromSynchronization;
            var actionBases = actions.Where(@base => excludedFromSynchronization[@base.Id] == null);
            return actionBases;
        }
    }
}
