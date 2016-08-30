using System.ComponentModel;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base.General;
using DevExpress.Utils;
using Xpand.Persistent.Base.General;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.TreeListEditors {
    [Description, EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class XpandTreeListEditorsModule : XpandModuleBase {
        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters){
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new XpandTreeListEditorServerModeUpdater());
        }
        protected override void RegisterEditorDescriptors(EditorDescriptorsFactory editorDescriptorsFactory) {
            editorDescriptorsFactory.RegisterListEditorAlias(EditorAliases.XpandTreeListEditor, typeof(ITreeNode), true);
        }
    }
}