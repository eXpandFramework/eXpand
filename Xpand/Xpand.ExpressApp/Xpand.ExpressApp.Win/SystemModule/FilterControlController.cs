using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Win.SystemModule {
    public interface IModelMemberFilterControl : IModelNode {
        [Category("eXpand")]
        [Description("Controls the ViewMode of the CriteriaProperty Editor")]
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
            View.GetItems<CriteriaPropertyEditor>().Each(editor => editor.Control.ViewMode = ((IModelPropertyEditorFilterControl)editor.Model).FilterEditorViewMode);
        }
    }
}