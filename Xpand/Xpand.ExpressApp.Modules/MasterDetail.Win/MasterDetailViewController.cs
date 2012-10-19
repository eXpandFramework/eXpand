using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.MasterDetail.Logic;
using Xpand.Persistent.Base.General;


namespace Xpand.ExpressApp.MasterDetail.Win {
    public class MasterDetailViewController : ExpressApp.Win.ListEditors.GridListEditors.GridView.MasterDetail.MasterDetailViewController, IMasterDetailViewController {
        #region Overrides of MasterDetailViewController
        public override bool IsMasterDetail() {
            return CreateRules(Frame).Any();
        }

        protected override List<MasterDetailRuleInfo> CreateRules(Frame frame) {
            return RequestRules.Invoke(frame);
        }
        #endregion
        #region Implementation of IMasterDetailViewController
        public Func<Frame, List<MasterDetailRuleInfo>> RequestRules { get; set; }
        #endregion
    }
}
