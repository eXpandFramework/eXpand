using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Navigation
{
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderHideFromNewMenu, "1=1", "1=1", Captions.HeaderHideFromNewMenu, Position.Top)]
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderHideFromNewMenu, "1=1", "1=1",
        Captions.ViewMessageHideFromNewMenu, Position.Bottom)]
    [NonPersistent]
    [HideFromNewMenu]
    [NavigationItem("Navigation/Hide From New Menu", "HideFromNewMenuObject_DetailView")]
    public class HideFromNewMenuObject
    {
    }
}
