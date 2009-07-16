using eXpand.ExpressApp.ModelArtifactState.Security.Permissions;
using System.Linq;
using eXpand.ExpressApp.Security.Controllers;

namespace eXpand.ExpressApp.ModelArtifactState.Security
{
    public partial class PopulateModulesController : PopulateController
    {
        public PopulateModulesController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof(ModelArtifactStatePermission);
        }


        protected override string GetPredefinedValues()
        {
            string ret = "";
            foreach (var module in Application.Modules.OrderBy(@base => @base.Name))
                ret += module.Name + ";";
            return ret.TrimEnd(';');
        }

        protected override string GetPermissionPropertyName()
        {
            return "Module";
        }
    }
}