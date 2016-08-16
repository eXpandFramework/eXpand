using Xpand.Persistent.Base.AdditionalViewControls;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider {
    public interface IAdditionalViewControl {
        IAdditionalViewControlsRule Rule { get; set; }
    }
}