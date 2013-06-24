using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.HtmlPropertyEditor.Web;
using DevExpress.ExpressApp.Model;
using System.Linq;

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
            var htmlEditor = htmlPropertyEditor.Editor;
            var modelHtmlEditor = ((IModelPropertyHtmlEditor) htmlPropertyEditor.Model).HtmlEditor;
            ApplyModel(modelHtmlEditor, htmlEditor, ApplyValues);
            if (htmlEditor != null)
                foreach (var toolbar in htmlEditor.Toolbars.ToList()) {
                    var modelHtmlEditorToolBar = modelHtmlEditor.ToolBars[toolbar.Name];
                    foreach (var toolbarItem in toolbar.Items.ToList()) {
                        var modelHtmlEditorToolBarItem = modelHtmlEditorToolBar.Items[toolbarItem.CommandName];
                        ApplyModel(modelHtmlEditorToolBarItem, toolbarItem,ApplyValues);
                    }
                }
        }

        public override void SynchronizeModel() {
        }
    }
}