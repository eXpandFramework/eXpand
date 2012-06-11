using System;
using System.ComponentModel;

using DevExpress.ExpressApp;

namespace SecurityDemo.Module.Web
{
    [ToolboxItemFilter("Xaf.Platform.Web")]
    public sealed partial class SecurityDemoAspNetModule : ModuleBase
    {
        protected override ModuleTypeList GetRequiredModuleTypesCore() {
            ModuleTypeList result = base.GetRequiredModuleTypesCore();
            result.Add(typeof(DevExpress.ExpressApp.Validation.ValidationModule));
            result.Add(typeof(DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase));
            result.Add(typeof(DevExpress.ExpressApp.TreeListEditors.Web.TreeListEditorsAspNetModule));
            return result;
        }
        public SecurityDemoAspNetModule()
        {
            InitializeComponent();
        }
    }
}
