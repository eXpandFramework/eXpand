using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Welcome
{
    
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderWelcome, "1=1", "1=1", Captions.HeaderWelcome, Position.Top)]
    [NonPersistent]
    [XpandNavigationItem("Welcome","WelcomeObject_DetailView")]
    [DisplayFeatureModel("WelcomeObject_DetailView","Welcome")]
    public class WelcomeObject
    {
    }
}
