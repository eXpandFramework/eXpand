using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.TreeListEditors.Web;
using DevExpress.Persistent.Base.General;
using DevExpress.Utils;
using Xpand.ExpressApp.TreeListEditors.Web.ListEditors;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.TreeNode;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.TreeListEditors.Web {
    [Description, EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(TreeListEditorsAspNetModule), "Resources.Toolbox_Module_TreeListEditors_Web.ico")]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabAspNetModules)]
    public sealed class XpandTreeListEditorsAspNetModule : XpandModuleBase {
        public XpandTreeListEditorsAspNetModule() {
            RequiredModuleTypes.Add(typeof(TreeListEditorsAspNetModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule));
            RequiredModuleTypes.Add(typeof(XpandTreeListEditorsModule));
        }

        protected override void RegisterEditorDescriptors(EditorDescriptorsFactory editorDescriptorsFactory) {
            editorDescriptorsFactory.RegisterListEditor(EditorAliases.XpandTreeListEditor, typeof(ITreeNode), typeof(XpandASPxTreeListEditor), true);
        }

        protected override IEnumerable<Type> GetDeclaredExportedTypes() {
            return base.GetDeclaredExportedTypes().Concat(new[] { typeof(ColumnChooserList) });
        }

    }
}