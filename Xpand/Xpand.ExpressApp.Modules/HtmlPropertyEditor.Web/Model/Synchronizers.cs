using DevExpress.ExpressApp.HtmlPropertyEditor.Web;
using DevExpress.ExpressApp.Model;
using System.Linq;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Web.ASPxHtmlEditor;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.HtmlPropertyEditor.Web.Model {
//    public class HtmlEditorModelSynchronizer : Persistent.Base.ModelAdapter.ModelSynchronizer<ASPxHtmlPropertyEditor, IModelPropertyEditor> {
//
//        public HtmlEditorModelSynchronizer(ASPxHtmlPropertyEditor htmlPropertyEditor)
//            : base(htmlPropertyEditor, (IModelPropertyEditor) htmlPropertyEditor.Model) {
//        }
//
//        protected override void ApplyModelCore() {
//            var htmlEditor = Control.Editor;
//            var modelHtmlEditor = ((IModelPropertyHtmlEditor)Model).HtmlEditor;
//            if (htmlEditor != null) {
//                var htmlEditorCustomDialog = new HtmlEditorCustomDialog();
//                foreach (var editor in ((IModelPropertyHtmlEditor)Model).HtmlEditorModelAdapters.SelectMany(adapter => adapter.ModelAdapters)) {
//                    ApplyModel(editor,htmlEditor , htmlEditorCustomDialog);
//                }
//                ApplyModel(modelHtmlEditor, htmlEditor,htmlEditorCustomDialog);
//                htmlEditor.CustomDialogs.Add(htmlEditorCustomDialog);
//            }
//        }
//
//        void ApplyModel(IModelHtmlEditor modelHtmlEditor, ASPxHtmlEditor htmlEditor, HtmlEditorCustomDialog htmlEditorCustomDialog) {
//            ApplyModel(modelHtmlEditor, htmlEditor, ApplyValues);
//            ApplyToolbarModel(modelHtmlEditor, htmlEditor,modelHtmlEditor.GetPropertyName(editor => editor.ToolBars));
//            ApplyShortcutModel(modelHtmlEditor, htmlEditor, modelHtmlEditor.GetPropertyName(editor => editor.Shortcuts));
//            ApplyCustomDialodModel(modelHtmlEditor, htmlEditor, modelHtmlEditor.GetPropertyName(editor => editor.CustomDialogs),htmlEditorCustomDialog);
//        }
//
//        void ApplyCustomDialodModel(IModelHtmlEditor modelHtmlEditor, ASPxHtmlEditor htmlEditor, string propertyName, HtmlEditorCustomDialog htmlEditorCustomDialog){
//            var editorCustomDialogs = (IModelHtmlEditorCustomDialogs)((ModelNode)modelHtmlEditor)[propertyName];
//            foreach (var modelCustomDialog in editorCustomDialogs.OfType<ModelNode>()) {
//                var editorCustomDialog = htmlEditor.CustomDialogs[modelCustomDialog.Id];
//                if (editorCustomDialog == null) {
//                    ApplyModel(modelCustomDialog, htmlEditorCustomDialog,ApplyValues);
//                }
//            }
//        }
//
//        void ApplyToolbarModel(IModelHtmlEditor modelHtmlEditor, ASPxHtmlEditor htmlEditor, string propertyName) {
//            int index = 0;
//            var toolBars = (IModelHtmlEditorToolBars)((ModelNode)modelHtmlEditor)[propertyName];
//            foreach (var toolbar in toolBars.ToList()) {
//                var editorToolBar = htmlEditor.Toolbars[toolbar.GetValue<string>("Id")];
//                ApplyModel(toolbar, editorToolBar, ApplyValues);
//                ApplyToolbarItemModel(toolbar, editorToolBar,index);
//                index++;
//            }
//        }
//
//        void ApplyToolbarItemModel(IModelHtmlEditorToolBar toolbar, HtmlEditorToolbar editorToolbar, int index) {
//            int i = 0;
//            foreach (var toolbarItem in toolbar.Items.ToList()) {
//                var editorToolBarItem =  EditorToolBarItem(toolbarItem, editorToolbar);
//                ApplyModel(toolbarItem, editorToolBarItem, ApplyValues);
//                var visibleIndex = (index + 1) * i;
//                if (editorToolBarItem.Visible && editorToolBarItem.VisibleIndex!=visibleIndex) {
//                    editorToolBarItem.VisibleIndex = visibleIndex;
//                }
//                i++;
//            }
//        }
//
//        HtmlEditorToolbarItem EditorToolBarItem(IModelHtmlEditorToolBarItem modelHtmlEditorToolBarItem, HtmlEditorToolbar editorToolbar) {
//            var commandName = modelHtmlEditorToolBarItem is IModelToolbarCustomDialogButton ? modelHtmlEditorToolBarItem.GetValue<string>("Name") : modelHtmlEditorToolBarItem.GetValue<string>("Id");
//            var editorToolbarItem = editorToolbar.Items.FirstOrDefault(item => item.CommandName == commandName);
//            if (editorToolbarItem == null) {
//                editorToolbarItem = new ToolbarCustomDialogButton();
//                editorToolbar.Items.Add(editorToolbarItem);
//            }
//            return editorToolbarItem;
//        }
//
//        void ApplyShortcutModel(IModelHtmlEditor modelHtmlEditor, ASPxHtmlEditor htmlEditor,string propertyName) {
//            var editorShortcuts = (IModelHtmlEditorShortcuts)((ModelNode)modelHtmlEditor)[propertyName];
//            foreach (var modelShortcut in editorShortcuts) {
//                var editorShortcut = new HtmlEditorShortcut();
//                htmlEditor.Shortcuts.Add(editorShortcut);
//                ApplyModel(modelShortcut, editorShortcut,ApplyValues);        
//            }
//        }
//
//        public override void SynchronizeModel() {
//        }
//    }
}