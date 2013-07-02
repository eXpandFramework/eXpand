using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.HtmlPropertyEditor.Web;
using DevExpress.ExpressApp.Model;
using System.Linq;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Web.ASPxHtmlEditor;

namespace Xpand.ExpressApp.HtmlPropertyEditor.Web.Model {
    public class HtmlEditorModelSynchronizer : Persistent.Base.ModelAdapter.ModelSynchronizer<DetailView, IModelDetailView> {
        public HtmlEditorModelSynchronizer(DetailView control)
            : base(control, control.Model) {
        }

        protected override void ApplyModelCore() {
            foreach (var htmlEditor in Control.GetItems<ASPxHtmlPropertyEditor>()) {
                htmlEditor.ControlCreated+=HtmlEditorOnControlCreated;
            }
        }

        void HtmlEditorOnControlCreated(object sender, EventArgs eventArgs) {
            var htmlPropertyEditor = ((ASPxHtmlPropertyEditor) sender);
            htmlPropertyEditor.ControlCreated -= HtmlEditorOnControlCreated;
            var htmlEditor = htmlPropertyEditor.Editor;
            var modelHtmlEditor = ((IModelPropertyHtmlEditor) htmlPropertyEditor.Model).HtmlEditor;
            if (htmlEditor != null) {
                ApplyModel(modelHtmlEditor, htmlEditor, ApplyValues);
                ApplyToolbarModel(modelHtmlEditor,htmlEditor);
                ApplyShortcutModel(modelHtmlEditor, htmlEditor);
                ApplyCustomDialodModel(modelHtmlEditor, htmlEditor);
            }
        }

        void ApplyCustomDialodModel(IModelHtmlEditor modelHtmlEditor, ASPxHtmlEditor htmlEditor) {
            foreach (var modelCustomDialog in modelHtmlEditor.CustomDialogs.OfType<ModelNode>()) {
                var editorCustomDialog = htmlEditor.CustomDialogs[modelCustomDialog.Id];
                if (editorCustomDialog == null) {
                    var htmlEditorCustomDialog = new HtmlEditorCustomDialog();
                    ApplyModel(modelCustomDialog, htmlEditorCustomDialog,ApplyValues);
                    htmlEditor.CustomDialogs.Add(htmlEditorCustomDialog);
                }
            }
        }

        void ApplyToolbarModel(IModelHtmlEditor modelHtmlEditor, ASPxHtmlEditor htmlEditor) {
            int index = 0;
            foreach (var toolbar in modelHtmlEditor.ToolBars.ToList()) {
                var editorToolBar = htmlEditor.Toolbars[toolbar.GetValue<string>("Id")];
                ApplyModel(toolbar, editorToolBar, ApplyValues);
                ApplyToolbarItemModel(toolbar, editorToolBar,index);
                index++;
            }
        }

        void ApplyToolbarItemModel(IModelHtmlEditorToolBar toolbar, HtmlEditorToolbar editorToolbar, int index) {
            int i = 0;
            foreach (var toolbarItem in toolbar.Items.ToList()) {
                var editorToolBarItem =  EditorToolBarItem(toolbarItem, editorToolbar);
                ApplyModel(toolbarItem, editorToolBarItem, ApplyValues);
                var visibleIndex = (index + 1) * i;
                if (editorToolBarItem.Visible && editorToolBarItem.VisibleIndex!=visibleIndex) {
                    editorToolBarItem.VisibleIndex = visibleIndex;
                }
                i++;
            }
        }

        HtmlEditorToolbarItem EditorToolBarItem(IModelHtmlEditorToolBarItem modelHtmlEditorToolBarItem, HtmlEditorToolbar editorToolbar) {
            var commandName = modelHtmlEditorToolBarItem is IModelToolbarCustomDialogButton ? modelHtmlEditorToolBarItem.GetValue<string>("Name") : modelHtmlEditorToolBarItem.GetValue<string>("Id");
            var editorToolbarItem = editorToolbar.Items.FirstOrDefault(item => item.CommandName == commandName);
            if (editorToolbarItem == null) {
                editorToolbarItem = new ToolbarCustomDialogButton();
                editorToolbar.Items.Add(editorToolbarItem);
            }
            return editorToolbarItem;
        }

        void ApplyShortcutModel(IModelHtmlEditor modelHtmlEditor, ASPxHtmlEditor htmlEditor) {
            foreach (var modelShortcut in modelHtmlEditor.Shortcuts) {
                var editorShortcut = new HtmlEditorShortcut();
                htmlEditor.Shortcuts.Add(editorShortcut);
                ApplyModel(modelShortcut, editorShortcut,ApplyValues);        
            }
        }

        public override void SynchronizeModel() {
        }
    }
}