using System;
using System.Linq;
using DevExpress.Data;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web;
using Xpand.ExpressApp.Model;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Web.SystemModule {
    public class UnboundColumnController : ExpressApp.Model.UnboundColumnController {
        protected override void AddColumn(IModelColumnUnbound modelColumnUnbound){
            var asPxGridListEditor = ((ASPxGridListEditor) View.Editor);
            modelColumnUnbound.PropertyName = modelColumnUnbound.Id;
            asPxGridListEditor.AddColumn(modelColumnUnbound);
            new UnboundColumnSynchronizer((ASPxGridListEditor)View.Editor, View.Model).ApplyModel();
        }

        protected override void OnActivated() {
            base.OnActivated();
            var gridListEditor = View.Editor as ASPxGridListEditor;
            if (gridListEditor != null){
                gridListEditor.ModelApplied +=GridListEditorOnModelApplied;
            }
            
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            var gridListEditor = View.Editor as ASPxGridListEditor;
            if (gridListEditor != null){
                gridListEditor.ModelApplied -=GridListEditorOnModelApplied;
            }
        }

        private void GridListEditorOnModelApplied(object sender, EventArgs eventArgs){
            new UnboundColumnSynchronizer((ASPxGridListEditor)sender, View.Model).ApplyModel();
        }    
    }

    public class UnboundColumnSynchronizer : ModelSynchronizer<ASPxGridListEditor, IModelListView> {
        public UnboundColumnSynchronizer(ASPxGridListEditor control, IModelListView model)
            : base(control, model) {
        }

        void ApplyModelCore(GridViewDataColumn columnWithInfo,IModelColumnUnbound modelColumnUnbound) {
            columnWithInfo.FieldName = $"U{modelColumnUnbound.Id}";
            columnWithInfo.UnboundType = UnboundColumnType.Decimal;
            columnWithInfo.UnboundExpression = modelColumnUnbound.UnboundExpression;
            columnWithInfo.PropertiesEdit = new ASPxPropertyEditorProperties(){DisplayFormatString = modelColumnUnbound.DisplayFormat};
        }

        protected override void ApplyModelCore() {
            ForEachColumnLink(ApplyModelCore);
        }

        public override void SynchronizeModel(){
            throw new NotImplementedException();
        }

        void ForEachColumnLink(Action<GridViewDataColumn,IModelColumnUnbound> action) {
            Model.Columns.OfType<IModelColumnUnbound>().Each(unbound => {
                var xafGridColumn = Control.Grid.Columns.OfType<GridViewDataColumn>().FirstOrDefault(column => column.Caption==unbound.Id);
                if (xafGridColumn != null&&xafGridColumn.Visible) action.Invoke(xafGridColumn,unbound);
            });
        }

    }
}