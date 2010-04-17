using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.Security.Permissions;

namespace eXpand.ExpressApp.Security.Controllers
{
    public partial class PopulateViewsController : PopulateController<LogicRulePermission>
    {
        public PopulateViewsController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override string GetPredefinedValues(IModelMember wrapper)
        {
            string ret = Application.Model.Views.Aggregate("", (current, view) => current + (view.Id + ";"));
            ret = ret.TrimEnd(';');
            return ret;
        }

        protected override Expression<Func<LogicRulePermission, object>> GetPropertyName()
        {
            return x=>x.ViewId;
        }
    }
}