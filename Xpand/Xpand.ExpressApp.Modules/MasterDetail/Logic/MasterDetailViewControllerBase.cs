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

        public event EventHandler<NeedsRuleArgs> NeedsRule;

        protected virtual void OnNeedsRule(NeedsRuleArgs e) {
            EventHandler<NeedsRuleArgs> handler = NeedsRule;
            if (handler != null) handler(this, e);
        }
    }

    public class NeedsRuleArgs : EventArgs {
        readonly List<IMasterDetailRule> _rules = new List<IMasterDetailRule>();


        public List<IMasterDetailRule> Rules {
            get {
                return _rules;
            }
        }

    }
}
