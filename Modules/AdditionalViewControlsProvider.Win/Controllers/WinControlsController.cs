using DevExpress.ExpressApp;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Controllers;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Controls;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Decorators;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Controllers
{
    public partial class WinControlsController : ViewController, IAdditionalViewControlsProvider
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            Frame.GetController<WinShowAdditionalViewControlsController>().Register(this, View, Frame);
            
        }
        protected override void OnDeactivating()
        {
            Frame.GetController<WinShowAdditionalViewControlsController>().Unregister(this);
            base.OnDeactivating();
        }
        public object CreateControl()
        {
            return new HintPanel();
        }

        public AdditionalViewControlsProviderDecorator DecorateControl(object control)
        {
            return new WinHintPanelDecorator(View, (HintPanel)control);
        }
    }
}