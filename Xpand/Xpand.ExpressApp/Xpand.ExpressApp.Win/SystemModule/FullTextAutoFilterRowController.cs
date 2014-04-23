using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.XtraGrid.Views.Grid;
using Xpand.ExpressApp.SystemModule.Search;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Xpo;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class FullTextAutoFilterRowController:ViewController<ListView>{
        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();
            var columnsListEditor = View.Editor as ColumnsListEditor;
            var gridView = columnsListEditor.GridView();
            gridView.ColumnFilterChanged+=GridViewOnColumnFilterChanged;
        }

        private void GridViewOnColumnFilterChanged(object sender, EventArgs eventArgs){
            var gridView = ((GridView) sender);
            var activeFilterCriteria = gridView.ActiveFilterCriteria;
            if (activeFilterCriteria!=null){
                var memberInfos = View.Model.GetFullTextMembers().Select(member => member.GetXpmemberInfo());
                if (memberInfos.Any()){
                    FullTextOperatorProcessor.Process(activeFilterCriteria, memberInfos.ToList());
                    gridView.ActiveFilterCriteria = activeFilterCriteria;
                }
            }
        }
    }
}
