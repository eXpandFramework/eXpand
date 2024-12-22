using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.MasterDetail.Logic;
using Xpand.Persistent.Base.General;
using System.ComponentModel;


namespace Xpand.ExpressApp.MasterDetail.Win {
    public class MasterDetailViewController : ExpressApp.Win.ListEditors.GridListEditors.GridView.MasterDetail.MasterDetailViewController, IMasterDetailViewController {
        #region Overrides of MasterDetailViewController
        public override bool IsMasterDetail() {
            if (CreateRules(Frame).Any()) return true;
            return false;
        }

        protected override List<MasterDetailRuleInfo> CreateRules(Frame frame) => RequestRules.Invoke(frame);

        #endregion
        #region Implementation of IMasterDetailViewController
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]

        #endregion
        #region Implementation of IMasterDetailViewController
        public Func<Frame, List<MasterDetailRuleInfo>> RequestRules { get; set; }
        #endregion
    }
}
