using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;

namespace FeatureCenter.Module.Welcome
{
    
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderWelcome, "1=1", "1=1", Captions.HeaderWelcome, Position.Top)]
    [NonPersistent]
    [eXpand.ExpressApp.Attributes.NavigationItem("Welcome","WelcomeObject_DetailView")]
    [DisplayFeatureModel("WelcomeObject_DetailView","Welcome")]
    public class WelcomeObject
    {
    }
}
