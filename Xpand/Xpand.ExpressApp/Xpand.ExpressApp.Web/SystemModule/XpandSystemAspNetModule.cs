using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Validation;
using DevExpress.ExpressApp.Web;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.Web.SystemModule {
    public interface IModelOptionsCollectionEditMode : IModelOptions {
        [DefaultValue(ViewEditMode.Edit)]
        [Category("eXpand")]
        ViewEditMode CollectionsEditMode { get; set; }
    }

    [ToolboxItem(true)]
    [DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
    [Description("Overrides Controllers from the SystemModule and supplies additional basic Controllers that are specific for ASP.NET applications.")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(XpandWebApplication), "Resources.WebSystemModule.ico")]
    public sealed class XpandSystemAspNetModule : XpandModuleBase {
        public XpandSystemAspNetModule() {
            RequiredModuleTypes.Add(typeof(XpandSystemModule));
            RequiredModuleTypes.Add(typeof(ValidationModule));
        }
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            if (Application != null) Application.LoggedOn += ApplicationOnLoggedOn;
        }

        void ApplicationOnLoggedOn(object sender, LogonEventArgs logonEventArgs) {
            ((ShowViewStrategy)Application.ShowViewStrategy).CollectionsEditMode = ((IModelOptionsCollectionEditMode)Application.Model.Options).CollectionsEditMode;
        }

        protected override IEnumerable<Type> GetDeclaredExportedTypes() {
            return new List<Type>();
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelOptions, IModelOptionsCollectionEditMode>();
        }
    }
}
