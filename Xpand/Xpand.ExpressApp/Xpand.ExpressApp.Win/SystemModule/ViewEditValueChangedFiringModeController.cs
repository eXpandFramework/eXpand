using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using Xpand.ExpressApp.SystemModule;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.Win.SystemModule {
    public interface IModelClassEditValueChangedFiringMode {
        [DefaultValue(EditValueChangedFiringMode.Buffered)]
        [Category(AttributeCategoryNameProvider.Xpand)]
        EditValueChangedFiringMode EditValueChangedFiringMode { get; set; }
    }

    [ModelInterfaceImplementor(typeof(IModelClassEditValueChangedFiringMode), "ModelClass")]
    public interface IModelListViewEditValueChangedFiringMode : IModelClassEditValueChangedFiringMode {

    }

    public class ViewEditValueChangedFiringModeController : ListViewController<ColumnsListEditor>, IModelExtender {
        GridView _mainView;

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (((IModelListViewEditValueChangedFiringMode)View.Model).EditValueChangedFiringMode == EditValueChangedFiringMode.Buffered) {
                var winColumnsListEditor = (View.Editor as WinColumnsListEditor);
                if (winColumnsListEditor != null) {
                    _mainView = winColumnsListEditor.ColumnView as GridView;
                    if (_mainView != null)
                        _mainView.ShownEditor += MainViewOnShownEditor;
                }
            }
        }
        void MainViewOnShownEditor(object sender, EventArgs args) {
            var view = (GridView)sender;
            if (view.IsFilterRow(view.FocusedRowHandle))
                view.ActiveEditor.Properties.EditValueChangedFiringMode = EditValueChangedFiringMode.Buffered;
        }


        #region IModelExtender Members

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelListView, IModelListViewEditValueChangedFiringMode>();
            extenders.Add<IModelClass, IModelClassEditValueChangedFiringMode>();
        }

        #endregion
    }
}