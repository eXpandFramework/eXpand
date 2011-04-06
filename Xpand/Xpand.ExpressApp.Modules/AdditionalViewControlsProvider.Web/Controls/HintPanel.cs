using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Web.Controls {

    public class HintPanel : HintPanelBase, IAdditionalViewControl {
        public IAdditionalViewControlsRule Rule {get;set;}
    }

}