using Xpand.Persistent.Base.AdditionalViewControls;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Web.Controls {

    public class HintPanel : HintPanelBase, IAdditionalViewControl {
        public IAdditionalViewControlsRule Rule {get;set;}
    }

}