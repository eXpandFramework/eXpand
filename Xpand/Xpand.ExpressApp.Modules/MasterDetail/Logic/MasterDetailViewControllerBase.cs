using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using Xpand.ExpressApp.Logic.Conditional.Logic;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.MasterDetail.Logic {
    public abstract class MasterDetailViewControllerBase : ViewController<ListView> {
        public Func<Frame, List<MasterDetailRuleInfo>> RequestRules { get; set; }
        

    }
}
