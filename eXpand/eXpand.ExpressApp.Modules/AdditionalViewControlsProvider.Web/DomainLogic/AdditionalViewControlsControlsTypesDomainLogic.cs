using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Web.Controls;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Web.Decorators;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Web.DomainLogic {
    [DomainLogic(typeof(IAdditionalViewControlsRule))]
    public class AdditionalViewControlsControlsTypesDomainLogic : AdditionalViewControlsProvider.DomainLogic.AdditionalViewControlsControlsTypesDomainLogic<HintPanel,WebHintPanelDecorator>
    {
        
    }
}