using System;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Views.Grid;
using Xpand.ExpressApp.SystemModule.Search;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Xpo;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class FullTextAutoFilterRowController:ViewController<ListView>{
        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();
            var columnsListEditor = View.Editor as WinColumnsListEditor;
            if (columnsListEditor != null){
                var gridView = columnsListEditor.ColumnView;
                if (gridView != null) gridView.ColumnFilterChanged+=GridViewOnColumnFilterChanged;
            }
        }

        private void GridViewOnColumnFilterChanged(object sender, EventArgs eventArgs){
            var gridView = ((GridView) sender);
            var activeFilterCriteria = gridView.ActiveFilterCriteria;
            if (!ReferenceEquals(activeFilterCriteria,null)){
                var memberInfos = View.Model.GetFullTextMembers().Select(member => member.GetXpmemberInfo()).ToArray();
                if (memberInfos.Any()){
                    var filterCriteria = FullTextOperatorProcessor.Process(activeFilterCriteria, memberInfos.ToList());
                    gridView.ActiveFilterCriteria = filterCriteria as CriteriaOperator;
                }
            }
        }
    }
}
