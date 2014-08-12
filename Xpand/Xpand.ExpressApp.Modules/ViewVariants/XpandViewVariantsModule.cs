using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Validation;
using DevExpress.ExpressApp.ViewVariantsModule;
using DevExpress.Utils;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Controllers.Actions;
using EditorBrowsableState = System.ComponentModel.EditorBrowsableState;

namespace Xpand.ExpressApp.ViewVariants {
    [Description(
        "Includes Property Editors and Controllers to DevExpress.ExpressApp.ViewVariants Module. Enables View Cloning"),
     EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(ViewVariantsModule), "Resources.Toolbox_Module_ViewVariants.ico")]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class XpandViewVariantsModule : XpandModuleBase, IModelXmlConverter, IModifyModelActionUser {
        public const string ViewVariantsModelCategory = "eXpand.ViewVariants";

        public XpandViewVariantsModule() {
            RequiredModuleTypes.Add(typeof(ViewVariantsModule));
            RequiredModuleTypes.Add(typeof(ConditionalAppearanceModule));
            RequiredModuleTypes.Add(typeof(ValidationModule));
        }

        public override void Setup(XafApplication application){
            base.Setup(application);
            application.UserDifferencesLoaded+=ApplicationOnUserDifferencesLoaded;
        }

        private void ApplicationOnUserDifferencesLoaded(object sender, EventArgs eventArgs){
            var modelApplicationBase = ((ModelApplicationBase) Application.Model);
            if (modelApplicationBase.GetLayer(ModifyVariantsController.ClonedViewsWithReset) == null){
                var modelApplication = modelApplicationBase.CreatorInstance.CreateModelApplication();
                modelApplication.Id = ModifyVariantsController.ClonedViewsWithReset;
                var xml = ((IModelApplicationViewVariants)Application.Model).ViewVariants.Storage;
                if (!string.IsNullOrEmpty(xml)){
                    new ModelXmlReader().ReadFromString(modelApplication, "", xml);
                    modelApplicationBase.AddLayerBeforeLast(modelApplication);
                }
            }
        }

        void IModelXmlConverter.ConvertXml(ConvertXmlParameters parameters) {
            ConvertXml(parameters);
            if (typeof(IModelListView).IsAssignableFrom(parameters.NodeType)&&parameters.ContainsKey("IsClonable")) {
                var value = parameters.Values["IsClonable"];
                parameters.Values.Remove("IsClonable");
                parameters.Values.Add(new KeyValuePair<string, string>("IsViewClonable",value));
            }
        }
    }
}