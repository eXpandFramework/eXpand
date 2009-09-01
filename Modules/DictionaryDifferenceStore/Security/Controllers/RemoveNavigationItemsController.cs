using System.Security;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base.Security;
using eXpand.ExpressApp.DictionaryDifferenceStore.BaseObjects;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.DictionaryDifferenceStore.Security.Controllers
{
    public partial class RemoveNavigationItemsController : BaseWindowController
    {
        public RemoveNavigationItemsController()
        {
            InitializeComponent();
            RegisterActions(components);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            var controller = Frame.GetController<ShowNavigationItemController>();
            controller.CustomShowNavigationItem += ControllerOnCustomShowNavigationItem;
                       
        }

        private void ControllerOnCustomShowNavigationItem(object sender, CustomShowNavigationItemEventArgs args)
        {
            var viewShortcut = args.ActionArguments.SelectedChoiceActionItem.Data as ViewShortcut;
            if (viewShortcut != null &&new ApplicationNodeWrapper(Application.Info).Views.FindViewById(viewShortcut.ViewId).ClassName==typeof(XpoModelDictionaryDifferenceStore).FullName)
            {
                SecuritySystem.ReloadPermissions();
                if (!SecuritySystem.IsGranted(new EditModelPermission(ModelAccessModifier.Allow)))
                {
                    args.Handled = true;
                    throw new SecurityException(ExceptionLocalizerTemplate<SystemExceptionResourceLocalizer, ExceptionId>.GetExceptionMessage(ExceptionId.PermissionIsDenied));
                }
            }
        }


        protected virtual bool HasEditModelPermission()
        {
            var basicUser = SecuritySystem.Instance as ISimpleUser;
            if (basicUser!= null)
                return basicUser.IsAdministrator;
            var user = SecuritySystem.CurrentUser as IUser;
            if (user != null)
                return SecuritySystem.Instance.IsGranted(new EditModelPermission(ModelAccessModifier.Allow));
            return false;
        }
    }
}