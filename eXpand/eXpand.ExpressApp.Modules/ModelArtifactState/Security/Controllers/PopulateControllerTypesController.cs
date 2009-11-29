using System;
using System.Linq.Expressions;
using DevExpress.ExpressApp.NodeWrappers;
using eXpand.ExpressApp.ModelArtifactState.Security.Permissions;
using eXpand.ExpressApp.Security.Controllers;
using System.Linq;

namespace eXpand.ExpressApp.ModelArtifactState.Security.Controllers
{
    public partial class PopulateControllerTypesController : PopulateController<ControllerStateRulePermission>
    {
        public PopulateControllerTypesController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void populate()
        {
            if (typeof(ControllerStateRulePermission).IsAssignableFrom(View.ObjectTypeInfo.Type))
                base.populate();
        }

        protected override string GetPredefinedValues(PropertyInfoNodeWrapper wrapper1)
        {
            string ret = "";
            foreach (
                var controllerInfoNodeWrapper in
                    new ApplicationNodeWrapper(Application.Info).ActionDesign.Controllers.AllControllers.OrderBy(wrapper => wrapper.Name))
                ret += controllerInfoNodeWrapper.Name + ";";
                
            ret = ret.TrimEnd(';');
            return ret;
        }

        protected override Expression<Func<ControllerStateRulePermission, object>> GetPropertyName()
        {
            return x=>x.ControllerType;
        }

    }
}