using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Win.Miscellaneous.ApplicationMultipleInstances
{
    [AdditionalViewControlsRule(Module.Captions.ViewMessage + " " + Captions.HeaderApplicationMultipleInstances, "1=1", "1=1",
        Captions.ViewMessageApplicationMultipleInstances, Position.Bottom)]
    [AdditionalViewControlsRule(Module.Captions.Header + " " + Captions.HeaderApplicationMultipleInstances, "1=1", "1=1",
        Captions.HeaderApplicationMultipleInstances, Position.Top)]
    [NonPersistent]
    [NavigationItem(Module.Captions.Miscellaneous+"Application Multiple Instances", "AMIObject_DetailView")]
    [DisplayFeatureModel("AMIObject_DetailView", "ApplicationMultipleInstances")]
    public class AMIObject {
        
    }
}
