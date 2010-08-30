using System.Security;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.Security.Core;

namespace eXpand.ExpressApp.ModelDifference.Security.Controllers
{
    public class NavigationItemsController : WindowController
    {
        [CoverageExclude]
        protected override void OnActivated()
        {
            base.OnActivated();
            Frame.GetController<ShowNavigationItemController>().CustomShowNavigationItem += ControllerOnCustomShowNavigationItem;    
        }

        [CoverageExclude]
        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            Frame.GetController<ShowNavigationItemController>().CustomShowNavigationItem -= ControllerOnCustomShowNavigationItem;
        }

        protected internal virtual void ControllerOnCustomShowNavigationItem(object sender, CustomShowNavigationItemEventArgs args)
        {
            if (args.FitToObjectType(Application, typeof(ModelDifferenceObject)))
            {
                SecuritySystem.ReloadPermissions();
                if (!SecuritySystemExtensions.IsGranted(new EditModelPermission(ModelAccessModifier.Allow),false))
                {
                    args.Handled = true;
                    throw new SecurityException(ExceptionLocalizerTemplate<SystemExceptionResourceLocalizer, ExceptionId>.GetExceptionMessage(ExceptionId.PermissionIsDenied));
                }
            }
        }
    }
}