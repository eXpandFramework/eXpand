using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.BaseImpl;

namespace eXpand.ExpressApp.Security.Controllers
{
    public partial class DefaultRolePermissionsController : ViewController
    {
        public DefaultRolePermissionsController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof (Role);
            TargetViewType=ViewType.DetailView;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            if (View.ObjectSpace.Session.IsNewObject(View.CurrentObject))
                ((Role) View.CurrentObject).AddPermission(new ObjectAccessPermission(typeof (object),
                                                                                     ObjectAccess.AllAccess));
        }
    }
}
