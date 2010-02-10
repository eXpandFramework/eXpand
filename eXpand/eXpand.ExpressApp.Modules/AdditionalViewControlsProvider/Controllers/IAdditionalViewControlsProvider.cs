using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Controllers
{
    public interface IAdditionalViewControlsProvider
    {
        object CreateControl();
        View View { get; }
        AdditionalViewControlsProviderDecorator DecorateControl(object control);
    }
}