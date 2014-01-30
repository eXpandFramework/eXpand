using System.ComponentModel;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Utils;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.TreeListEditors {
    [Description, EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class XpandTreeListEditorsModule : XpandModuleBase {
        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters){
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new XpandTreeListEditorServerModeUpdater());
        }
    }
}