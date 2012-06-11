using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using FeatureCenter.Module.Win;

namespace SecurityDemo.Module.Win
{
    [ToolboxItemFilter("Xaf.Platform.Win")]
    public sealed partial class SecurityDemoWindowsFormsModule : ModuleBase
    {
        protected override ModuleTypeList GetRequiredModuleTypesCore() {
            ModuleTypeList result =base.GetRequiredModuleTypesCore();
            result.Add(typeof(DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase));
            result.Add(typeof(DevExpress.ExpressApp.TreeListEditors.Win.TreeListEditorsWindowsFormsModule));
            result.Add(typeof(DevExpress.ExpressApp.Validation.ValidationModule));
            result.Add(typeof(DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule));
            return result;
        }
        public SecurityDemoWindowsFormsModule()
        {
            InitializeComponent();
        }
        public override ICollection<Type> GetXafResourceLocalizerTypes() {
            ICollection<Type> result = new List<Type>();
            result.Add(typeof(FeatureCenterMainFormTemplateLocalizer));
            result.Add(typeof(FeatureCenterPopupFormTemplateLocalizer));
            return result;
        }
    }
}
