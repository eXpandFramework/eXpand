using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Win.LogOut {
    [AdditionalViewControlsRule(Module.Captions.ViewMessage + " " + Captions.HeaderLogOut, "1=1", "1=1",
        Captions.ViewMessageLogOut, Position.Bottom)]
    [AdditionalViewControlsRule(Module.Captions.Header + " " + Captions.HeaderLogOut, "1=1", "1=1", Captions.HeaderLogOut
        , Position.Top)]
    [NonPersistent]
    [NavigationItem("Navigation/Log Out", "LOCustomer_DetailView")]
    public class LOCustomer  {
        
    }
}