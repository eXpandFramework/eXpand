using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.AdditionalViewControlsProvider.DomainLogic;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Controls;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Decorators;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Win.DomainLogic {
    [DomainLogic(typeof (IAdditionalViewControlsRule))]
    public class AdditionalViewControlsControlsTypesDomainLogic :AdditionalViewControlsControlsTypesDomainLogic<HintPanel, WinFrameControlDecorator> {
    }
}