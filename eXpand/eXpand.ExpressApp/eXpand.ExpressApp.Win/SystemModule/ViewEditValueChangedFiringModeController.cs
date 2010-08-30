using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.Win.SystemModule {
    public interface IModelClassEditValueChangedFiringMode
    {
        [DefaultValue(EditValueChangedFiringMode.Buffered)]
        [Category("eXpand")]
        EditValueChangedFiringMode EditValueChangedFiringMode { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassEditValueChangedFiringMode), "ModelClass")]
    public interface IModelListViewEditValueChangedFiringMode : IModelClassEditValueChangedFiringMode
    {

    }
    public class ViewEditValueChangedFiringModeController:ListViewController<GridListEditor>,IModelExtender
    {
        XafGridView mainView;

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            if (((IModelListViewEditValueChangedFiringMode)View.Model).EditValueChangedFiringMode==EditValueChangedFiringMode.Buffered) {
                mainView = ((GridListEditor)View.Editor).GridView;
                mainView.ShownEditor += MainViewOnShownEditor;
            }
        }
        void MainViewOnShownEditor(object sender, EventArgs args)
        {
            var view = (GridView)sender;
            if (view.IsFilterRow(view.FocusedRowHandle))
                view.ActiveEditor.Properties.EditValueChangedFiringMode = EditValueChangedFiringMode.Buffered;
        }


        #region IModelExtender Members

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelListView, IModelListViewEditValueChangedFiringMode>();
            extenders.Add<IModelClass, IModelClassEditValueChangedFiringMode>();
        }

        #endregion
    }
}