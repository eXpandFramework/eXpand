using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;

namespace Xpand.ExpressApp.Win.SystemModule {
    public interface IModelMemberFilterControl : IModelNode {
        [Category("eXpand")]
        [Description("Controls the ViewMode of the CriteriaProperty Editor")]
        [DefaultValue(FilterEditorViewMode.TextAndVisual)]
        FilterEditorViewMode FilterEditorViewMode { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelMemberFilterControl), "ModelMember")]
    public interface IModelPropertyEditorFilterControl : IModelMemberFilterControl {

    }
    public class FilterControlController : ViewController<DetailView>, IModelExtender {
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelMember, IModelMemberFilterControl>();
            extenders.Add<IModelPropertyEditor, IModelPropertyEditorFilterControl>();
        }
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            foreach (var editor in View.GetItems<CriteriaPropertyEditor>()) {
                if (editor.Control != null)
                    this.SetViewMode(editor); 
                else
                    editor.ControlCreated += Editor_ControlCreated;
            }
        }

        private void Editor_ControlCreated(object sender, EventArgs e) {
            this.SetViewMode((CriteriaPropertyEditor)sender);
        }

        private void SetViewMode(CriteriaPropertyEditor editor) {
            editor.Control.ViewMode = ((IModelPropertyEditorFilterControl)editor.Model).FilterEditorViewMode;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            foreach (var editor in View.GetItems<CriteriaPropertyEditor>()) {
                editor.ControlCreated -= this.Editor_ControlCreated;
            }
        }
    }
}