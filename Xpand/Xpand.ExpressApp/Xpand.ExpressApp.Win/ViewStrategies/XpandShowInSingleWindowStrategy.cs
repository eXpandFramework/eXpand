using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;

namespace Xpand.ExpressApp.Win.ViewStrategies {

    public class XpandShowInMultipleWindowsStrategy : ShowInMultipleWindowsStrategy {
        public XpandShowInMultipleWindowsStrategy(XafApplication application) : base(application) {
        }
        protected override bool IsNewWindowForced(ShowViewParameters parameters, ShowViewSource showViewSource) {
            var isNewWindowForced = base.IsNewWindowForced(parameters, showViewSource);
            return !isNewWindowForced && parameters.NewWindowTarget == NewWindowTarget.Separate || isNewWindowForced;
        }
    }
}
