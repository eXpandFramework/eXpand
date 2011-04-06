using Xpand.ExpressApp.AdditionalViewControlsProvider.Editors;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Web.Controls {
    public class WarningPanel : HintPanelBase, ISupportLayoutManager, IAdditionalViewControl {
        object ISupportLayoutManager.LayoutItem { get; set; }

        public IAdditionalViewControlsRule Rule { get; set; }
    }
}