using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.SystemModule {

    public interface IModelClassEditMode : IModelNode {
        [Category("eXpand")]
        [Description("Control detail view default edit mode")]
        ViewEditMode? ViewEditMode { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassEditMode), "ModelClass")]
    public interface IModelViewEditMode : IModelClassEditMode {

    }

    public class ViewEditModeController : ViewController<DetailView>, IModelExtender {

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var viewEditMode = ((IModelViewEditMode)View.Model).ViewEditMode;
            if (viewEditMode.HasValue && !(ObjectSpace.IsNewObject(View.CurrentObject)) && View.ViewEditMode != viewEditMode.Value) {
                UpdateViewEditModeState(viewEditMode.Value);
                UpdateViewAllowEditState();
            }
        }

        protected virtual void UpdateViewEditModeState(ViewEditMode viewEditMode) {
            View.ViewEditMode = viewEditMode;
        }

        protected virtual void UpdateViewAllowEditState() {
            View.AllowEdit["ViewEditMode"] = View.ViewEditMode == ViewEditMode.Edit;
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelClass, IModelClassEditMode>();
            extenders.Add<IModelListView, IModelViewEditMode>();
            extenders.Add<IModelDetailView, IModelViewEditMode>();
        }
    }
}