using System;
using System.Linq.Expressions;
using DevExpress.ExpressApp.NodeWrappers;
using eXpand.ExpressApp.ModelArtifactState.ActionState.Logic;
using eXpand.ExpressApp.Security.Controllers;
using System.Linq;

namespace eXpand.ExpressApp.ModelArtifactState.ActionState
{
    public partial class PopulateActionsController : PopulateController<ActionStateRulePermission>
    {
        public PopulateActionsController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override string GetPredefinedValues(PropertyInfoNodeWrapper wrapper1)
        {
            string ret = new ApplicationNodeWrapper(Application.Info).ActionDesign.Actions.List.OrderBy(wrapper => wrapper.Id).Aggregate("", (current, action) => current + (action.Id + ";"));
            return ret.TrimEnd(';');
        }

        protected override Expression<Func<ActionStateRulePermission, object>> GetPropertyName()
        {
            return x=>x.ActionId;
        }
    }
}