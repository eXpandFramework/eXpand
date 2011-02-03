using Xpand.ExpressApp.AdditionalViewControlsProvider.Editors;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Web.Controls {
    [AdditionalViewControl]
    public class WarningPanel : HintPanelBase, ISupportLayoutManager {
        object ISupportLayoutManager.LayoutItem { get; set; }
    }
}