using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.MasterDetail.Logic {
    public interface IMasterDetailViewController {
        Func<Frame, List<MasterDetailRuleInfo>> RequestRules { get; set; }
    }
}
