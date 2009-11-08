using System;
using System.Linq.Expressions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using eXpand.ExpressApp.ModelArtifactState.Security.Permissions;
using eXpand.ExpressApp.Security.Controllers;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.ModelArtifactState.Security
{
    public partial class PopulateModulesController : PopulateController<ArtifactStateRulePermission>
    {
        public PopulateModulesController()
        {
            InitializeComponent();
            RegisterActions(components);
//            TargetObjectType = typeof(ArtifactStateRulePermission);
        }


        protected override string GetPredefinedValues(PropertyInfoNodeWrapper wrapper)
        {
            string ret = "";
            foreach (DictionaryNode node in Application.Info.GetChildNode(ModuleController.Modules).ChildNodes)
                ret += node.GetAttributeValue("Name") + ";";
            return ret.TrimEnd(';');
        }

        protected override Expression<Func<ArtifactStateRulePermission, object>> GetPropertyName()
        {
            return x=>x.Module;
        }
    }
}