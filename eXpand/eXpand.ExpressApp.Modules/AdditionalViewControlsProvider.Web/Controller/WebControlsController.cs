using DevExpress.ExpressApp;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Controllers;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Web.Controls;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Web.Decorators;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Web.Controller
{
    public partial class WebControlsController : ViewController, IAdditionalViewControlsProvider
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            Frame.GetController<WebShowAdditionalViewControlsController>().Register(this, View, Frame);
        }
        protected override void OnDeactivating()
        {
            Frame.GetController<WebShowAdditionalViewControlsController>().Unregister(this);
            base.OnDeactivating();
        }

        public object CreateControl()
        {
            return new HintPanel();
        }

        public AdditionalViewControlsProviderDecorator DecorateControl(object control)
        {
            return new WebHintPanelDecorator(View, (HintPanel) control);
        }
    }
}