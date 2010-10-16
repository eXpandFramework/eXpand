using System.ComponentModel;
using DevExpress.Utils;
using Xpand.ExpressApp.TreeListEditors.Win.Core;

namespace Xpand.ExpressApp.TreeListEditors.Win {
    [Description(
        "Includes Property Editors and Controllers to DevExpress.ExpressApp.TreeListEditors.Win Module.Enables recursive filtering"
        ), ToolboxTabName("eXpressApp"), EditorBrowsable(EditorBrowsableState.Always), Browsable(true),
     ToolboxItem(true)]
    public sealed partial class XpandTreeListEditorsWinModule : XpandModuleBase {
        public XpandTreeListEditorsWinModule() {
            InitializeComponent();
        }

        public override void AddGeneratorUpdaters(DevExpress.ExpressApp.Model.Core.ModelNodesGeneratorUpdaters updaters)
        {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new XpandTreeListEditorNodeGeneratorUpdater());
        }
    }
}