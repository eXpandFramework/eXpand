using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Win.TrayIcon
{
    [AdditionalViewControlsRule(Module.Captions.ViewMessage + " " + Captions.HeaderTrayIcon, "1=1", "1=1",
        Captions.ViewMessageTrayIcon, Position.Bottom)]
    [AdditionalViewControlsRule(Module.Captions.Header + " " + Captions.HeaderTrayIcon, "1=1", "1=1",
        Captions.HeaderTrayIcon, Position.Top)]
    [NonPersistent]
    [NavigationItem("Tray Icon", "TICustomer_DetailView")]
    [DisplayFeatureModel("TICustomer_DetailView", "TrayIcon")]
    public class TICustomer
    {
    }
}
