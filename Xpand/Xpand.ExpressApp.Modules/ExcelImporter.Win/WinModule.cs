using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Utils;
using Xpand.ExpressApp.Win.PropertyEditors.StringPropertyEditors;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.ExcelImporter.Win {
    [ToolboxBitmap(typeof(ExcelImporterWinModule))]
     [ToolboxItem(true)]
     [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed partial class ExcelImporterWinModule : ModuleBase {
        public ExcelImporterWinModule() {
            InitializeComponent();
        }

        protected override void RegisterEditorDescriptors(EditorDescriptorsFactory editorDescriptorsFactory){
            base.RegisterEditorDescriptors(editorDescriptorsFactory);
            editorDescriptorsFactory.List.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.StringLookupPropertyEditor, typeof(string), false)));
            editorDescriptorsFactory.List.Add(new PropertyEditorDescriptor(
                new EditorTypeRegistration(EditorAliases.StringLookupPropertyEditor, typeof(string),
                    typeof(StringLookupPropertyEditor), false)));
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            CalculatedPersistentAliasHelper.CustomizeTypesInfo(typesInfo);
        }
    }
}
