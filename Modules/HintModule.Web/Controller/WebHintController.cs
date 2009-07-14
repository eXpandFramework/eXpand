using DevExpress.ExpressApp;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.Web.SystemModule
{
    public class WebHintController : ViewController, IAdditionalInfoControlProvider
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            Frame.GetController<WebShowAdditionalInfoController>().Register(this);
        }
        protected override void OnDeactivating()
        {
            Frame.GetController<WebShowAdditionalInfoController>().Unregister(this);
            base.OnDeactivating();
        }
        public object CreateControl()
        {
            HintPanel hintPanel = new HintPanel();
            new WebHintDecorator(View, hintPanel);
            return hintPanel;
        }
    }
}
