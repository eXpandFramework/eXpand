using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.ExpressApp.NodeWrappers;
using eXpand.ExpressApp.Security.Permissions;

namespace eXpand.ExpressApp.Security.Controllers
{
    public partial class PopulateViewsController : PopulateController<LogicRulePermission>
    {
        public PopulateViewsController()
        {
            InitializeComponent();
            RegisterActions(components);
//            TargetObjectType = typeof (StatePermission);
        }


        protected override string GetPredefinedValues(PropertyInfoNodeWrapper wrapper)
        {
            string ret = new ApplicationNodeWrapper(Application.Info).Views.Items.Aggregate("", (current, view) => current + (view.Id + ";"));
            ret = ret.TrimEnd(';');
            return ret;
        }

        protected override Expression<Func<LogicRulePermission, object>> GetPropertyName()
        {
            return x=>x.ViewId;
        }
    }
}