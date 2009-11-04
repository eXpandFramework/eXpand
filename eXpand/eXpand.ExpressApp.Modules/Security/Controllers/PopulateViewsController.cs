using DevExpress.ExpressApp.NodeWrappers;
using eXpand.ExpressApp.Security.Permissions;

namespace eXpand.ExpressApp.Security.Controllers
{
    public partial class PopulateViewsController : PopulateController
    {
        public PopulateViewsController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof (StatePermission);
        }


        protected override string GetPredefinedValues(PropertyInfoNodeWrapper wrapper)
        {
            string ret = "";
            foreach (var view in new ApplicationNodeWrapper(Application.Info).Views.Items)
                ret += view.Id + ";";
            ret = ret.TrimEnd(';');
            return ret;
        }

        protected override string GetPermissionPropertyName()
        {
            return "ViewId";
        }
    }
}