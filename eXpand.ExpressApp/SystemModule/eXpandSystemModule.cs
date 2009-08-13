using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.Core.DictionaryHelpers;

namespace eXpand.ExpressApp.SystemModule
{
    [ToolboxItem(true)]
    [Description("Includes Controllers that represent basic features for XAF applications.")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof (XafApplication), "Resources.SystemModule.ico")]
    public sealed partial class eXpandSystemModule : ModuleBase
    {

        public override void UpdateModel(Dictionary model)
        {
            base.UpdateModel(model);
            new ApplicationNodeWrapper(model).Node.GetChildNode("Options").SetAttribute("UseServerMode", "True");
        }

        public override void ValidateModel(Dictionary model)
        {
            base.ValidateModel(model);
            DictionaryHelper.AddFields(model.RootNode, XafTypesInfo.XpoTypeInfoSource.XPDictionary);
        }

        
    }
}