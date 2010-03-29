using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.Core.DictionaryHelpers;

namespace eXpand.ExpressApp.SystemModule
{
    [ToolboxItem(true)]
    [Description("Includes Controllers that represent basic features for XAF applications.")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(XafApplication), "Resources.SystemModule.ico")]
    public sealed partial class eXpandSystemModule : ModuleBase
    {

        public override Schema GetSchema()
        {
            return new Schema(new DictionaryXmlReader().ReadFromResource(
                GetType().Assembly, "Resources.CommonSystemModuleSchema.xml"));
        }

        public override void ValidateModel(Dictionary model)
        {
            base.ValidateModel(model);
            DictionaryHelper.AddFields(model.RootNode, XafTypesInfo.XpoTypeInfoSource.XPDictionary);
        }
        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            application.SetupComplete += (sender, args) => DictionaryHelper.AddFields(application.Info, application.ObjectSpaceProvider.XPDictionary);
            application.LoggedOn += (sender, args) => DictionaryHelper.AddFields(application.Info, application.ObjectSpaceProvider.XPDictionary);
        }

    }
}