using System.ComponentModel;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base.General;
using DevExpress.Utils;

namespace Xpand.ExpressApp.TreeListEditors.Win
{
    [Description(
        "Includes Property Editors and Controllers to DevExpress.ExpressApp.TreeListEditors.Win Module.Enables recursive filtering"
        ), ToolboxTabName("eXpressApp"), EditorBrowsable(EditorBrowsableState.Always), Browsable(true),
     ToolboxItem(true)]
    public sealed partial class XpandTreeListEditorsWinModule : XpandModuleBase
    {
        public XpandTreeListEditorsWinModule()
        {
            InitializeComponent();
        }
        protected override void RegisterEditorDescriptors(System.Collections.Generic.List<EditorDescriptor> editorDescriptors)
        {
            base.RegisterEditorDescriptors(editorDescriptors);
            editorDescriptors.Add(new ListEditorDescriptor(new AliasAndEditorTypeRegistration(EditorAliases.CategorizedListEditor, typeof(ICategorizedItem), true, typeof(XpandCategorizedListEditor), true)));
        }

    }
}