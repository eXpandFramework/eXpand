using DevExpress.Xpo;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Win.Navigation.LogOut {
    [AdditionalViewControlsRule(Module.Captions.ViewMessage + " " + Captions.HeaderLogOut, "1=1", "1=1",
        Captions.ViewMessageLogOut, Position.Bottom)]
    [AdditionalViewControlsRule(Module.Captions.Header + " " + Captions.HeaderLogOut, "1=1", "1=1", Captions.HeaderLogOut
        , Position.Top)]
    [NonPersistent]
    [XpandNavigationItem("Navigation/Log Out", "FeatureCenter.Module.Win.Navigation.LogOut.LOCustomer_DetailView")]
    [DisplayFeatureModel("FeatureCenter.Module.Win.Navigation.LogOut.LOCustomer_DetailView", "LogOut")]
    public class LOCustomer  {
        
    }
}