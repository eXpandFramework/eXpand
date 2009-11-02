using System;
using System.Linq.Expressions;
using DevExpress.ExpressApp.NodeWrappers;
using eXpand.ExpressApp.ModelArtifactState.Security.Permissions;
using eXpand.ExpressApp.Security.Controllers;
using System.Linq;

namespace eXpand.ExpressApp.ModelArtifactState.Security.Controllers
{
    public partial class PopulateActionsController : PopulateController<ActionStateRulePermission>
    {
        public PopulateActionsController()
        {
            InitializeComponent();
            RegisterActions(components);
//            TargetObjectType = typeof(ActionStateRulePermission);
        }

        protected override string GetPredefinedValues(PropertyInfoNodeWrapper wrapper1)
        {
            string ret = "";
            foreach (var action in new ApplicationNodeWrapper(Application.Info).ActionDesign.Actions.List.OrderBy(wrapper => wrapper.Id))
                ret += action.Id + ";";
            return ret.TrimEnd(';');
        }

        protected override Expression<Func<ActionStateRulePermission, object>> GetPropertyName()
        {
            return x=>x.ActionId;
        }
    }
}