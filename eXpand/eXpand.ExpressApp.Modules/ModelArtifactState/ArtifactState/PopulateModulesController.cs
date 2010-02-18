using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.ExpressApp.NodeWrappers;
using eXpand.ExpressApp.Security.Controllers;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.ModelArtifactState.ArtifactState
{
    public partial class PopulateModulesController : PopulateController<ArtifactStateRulePermission>
    {
        public PopulateModulesController()
        {
            InitializeComponent();
            RegisterActions(components);
        }


        protected override string GetPredefinedValues(PropertyInfoNodeWrapper wrapper)
        {
            string ret = Application.Info.GetChildNode(ModuleController.Modules).ChildNodes.Aggregate("", (current, node) => current + (node.GetAttributeValue("Name") + ";"));
            return ret.TrimEnd(';');
        }

        protected override Expression<Func<ArtifactStateRulePermission, object>> GetPropertyName()
        {
            return x=>x.Module;
        }
    }
}