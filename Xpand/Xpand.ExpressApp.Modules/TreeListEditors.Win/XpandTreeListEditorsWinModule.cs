using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Updating;
using DevExpress.Utils;

namespace Xpand.ExpressApp.TreeListEditors.Win {
    [Description(
        "Includes Property Editors and Controllers to DevExpress.ExpressApp.TreeListEditors.Win Module.Enables recursive filtering"
        ), ToolboxTabName("eXpressApp"), EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(XpandTreeListEditorsWinModule))]
    [ToolboxItem(true)]
    public sealed partial class XpandTreeListEditorsWinModule : XpandModuleBase, IModelXmlConverter {
        public XpandTreeListEditorsWinModule() {
            InitializeComponent();
        }


        public void ConvertXml(ConvertXmlParameters parameters) {
            if (typeof(IModelListView).IsAssignableFrom(parameters.NodeType) && parameters.Values.ContainsKey("EditorTypeName")) {
                if (parameters.Values["EditorTypeName"] == "Xpand.ExpressApp.TreeListEditors.Win.XpandCategorizedListEditor")
                    parameters.Values["EditorTypeName"] ="Xpand.ExpressApp.TreeListEditors.Win.ListEditor.XpandCategorizedListEditor";
            }
        }
    }
}