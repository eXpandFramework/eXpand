using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.Core.DictionaryHelpers;

namespace eXpand.ExpressApp.SystemModule {
    [ToolboxItem(true)]
    [Description("Includes Controllers that represent basic features for XAF applications.")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof (XafApplication), "Resources.SystemModule.ico")]
    public sealed class eXpandSystemModule : ModuleBase {
        
        public override void Setup(XafApplication application) {
            base.Setup(application);
            application.SetupComplete +=
                (sender, args) =>
                DictionaryHelper.AddFields(application.Model, application.ObjectSpaceProvider.XPDictionary);
            application.LoggedOn +=
                (sender, args) =>
                DictionaryHelper.AddFields(application.Model, application.ObjectSpaceProvider.XPDictionary);
        }


    
    }

}