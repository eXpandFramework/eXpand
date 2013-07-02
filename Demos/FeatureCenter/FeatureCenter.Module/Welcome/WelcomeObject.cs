using DevExpress.Xpo;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Welcome
{
    
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderWelcome, "1=1", "1=1", Captions.HeaderWelcome, Position.Top)]
    [NonPersistent]
    [XpandNavigationItem("Welcome", "FeatureCenter.Module.Welcome.WelcomeObject_DetailView")]
    [DisplayFeatureModel("FeatureCenter.Module.Welcome.WelcomeObject_DetailView", "Welcome")]
    public class WelcomeObject
    {
    }
}
