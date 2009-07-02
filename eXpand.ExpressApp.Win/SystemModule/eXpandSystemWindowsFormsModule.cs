using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.Win;
using eXpand.ExpressApp.Win.ListEditors;
using eXpand.ExpressApp.Win.PropertyEditors;

namespace eXpand.ExpressApp.Win.SystemModule {
    [ToolboxItem(true)]
    [DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
    [Description("Overrides Controllers from the SystemModule and supplies additional basic Controllers that are specific for Windows Forms XAFPoint applications.")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(WinApplication), "Resources.WinSystemModule.ico")]
    [ToolboxItemFilter("Xaf.Platform.Win")]
    public sealed partial class eXpandSystemWindowsFormsModule : ModuleBase {
        public eXpandSystemWindowsFormsModule() {
            InitializeComponent();
        }

        public override void UpdateModel(Dictionary model)
        {
            base.UpdateModel(model);
            model.RootNode.GetChildNodeByPath(
                "\\DetailViewItems\\PropertyEditor\\DefaultEditor[@PropertyType='DevExpress.Xpo.IXPSimpleObject']").
                SetAttribute("EditorType", typeof(LookupPropertyEditor).FullName);
            new ApplicationNodeWrapper(model).Views.Node.SetAttribute("DefaultListEditor", typeof(GridListEditor).FullName);
        }

        public override Schema GetSchema()
        {
            const string CommonTypeInfos = @"<Element Name=""Application"">
                        <Attribute Name=""NotifyIcon"" Choice=""False,True""/>
                        <Attribute Name=""CanClose"" Choice=""False,True""/>
                    </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(CommonTypeInfos));
        }
    }
}
