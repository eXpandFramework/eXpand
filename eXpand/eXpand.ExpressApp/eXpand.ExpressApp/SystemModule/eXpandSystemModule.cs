using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.Core.DictionaryHelpers;

namespace eXpand.ExpressApp.SystemModule {
    [ToolboxItem(true)]
    [Description("Includes Controllers that represent basic features for XAF applications.")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof (XafApplication), "Resources.SystemModule.ico")]
    public sealed class eXpandSystemModule : ModuleBase {
        static ModelApplicationCreatorProperties _modelApplicationCreatorProperties;
        static IValueManager<ModelApplicationCreatorProperties> _valueManager;
        public override void Setup(XafApplication application) {
            base.Setup(application);
            application.SetupComplete +=
                (sender, args) =>
                DictionaryHelper.AddFields(application.Model, application.ObjectSpaceProvider.XPDictionary);
            application.LoggedOn +=
                (sender, args) =>
                DictionaryHelper.AddFields(application.Model, application.ObjectSpaceProvider.XPDictionary);
        }

        protected override void CustomizeModelApplicationCreatorProperties(
            ModelApplicationCreatorProperties creatorProperties) {
            base.CustomizeModelApplicationCreatorProperties(creatorProperties);
            _modelApplicationCreatorProperties = creatorProperties;
        }

        public static ModelApplicationCreatorProperties ModelApplicationCreatorProperties {
            get {
                if (_valueManager == null) {
                    _valueManager = ValueManager.CreateValueManager<ModelApplicationCreatorProperties>();
                }
                return _valueManager.Value ?? (_valueManager.Value = _modelApplicationCreatorProperties);
            }
        }
    
    }

}