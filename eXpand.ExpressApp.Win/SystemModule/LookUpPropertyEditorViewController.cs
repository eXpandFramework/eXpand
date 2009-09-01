using System;
using System.Diagnostics;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public partial class LookUpPropertyEditorViewController : ViewController
    {
        public LookUpPropertyEditorViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetViewType=ViewType.ListView;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            View.ControlsCreated+=ViewOnControlsCreated;
        }

        private void ViewOnControlsCreated(object sender, EventArgs args)
        {
            if (View.Control is GridControl && ((GridControl) View.Control).MainView is GridView)
                ((GridControl) View.Control).HandleCreated+=OnHandleCreated;
        }

        private void OnHandleCreated(object sender, EventArgs args)
        {
            ((GridView) ((GridControl) sender).MainView).CustomRowCellEdit+=OnCustomRowCellEdit;
        }

        private void OnCustomRowCellEdit(object sender, CustomRowCellEditEventArgs args)
        {
            if (args.RepositoryItem is RepositoryItemLookupEdit)
                Debug.Print("");
        }
    }
}
