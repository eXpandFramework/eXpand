using DevExpress.ExpressApp;
using eXpand.ExpressApp.HintModule.Controllers;
using eXpand.ExpressApp.Core;

namespace eXpand.ExpressApp.HintModule.Web.Controller
{
    public partial class WebHintController : ViewController, IAdditionalInfoControlProvider
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            if (View is DetailView || !((ListView)View).IsNested(Frame))
                Frame.GetController<WebShowAdditionalInfoController>().Register(this);
        }
        protected override void OnDeactivating()
        {
            Frame.GetController<WebShowAdditionalInfoController>().Unregister(this);
            base.OnDeactivating();
        }
        public object CreateControl()
        {
            var hintPanel = new HintPanel();
            new WebHintDecorator(View, hintPanel);
            return hintPanel;
        }
    }
}