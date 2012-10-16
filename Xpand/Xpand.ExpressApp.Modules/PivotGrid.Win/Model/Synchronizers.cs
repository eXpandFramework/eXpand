using DevExpress.ExpressApp.PivotGrid.Win;
using System.Linq;
using DevExpress.XtraEditors.Repository;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.PivotGrid.Win.Model {
    public class PivotGridListEditorModelSynchronizer : ModelListSynchronizer {
        public PivotGridListEditorModelSynchronizer(PivotGridListEditor columnViewEditor)
            : base(columnViewEditor, columnViewEditor.Model) {
            ModelSynchronizerList.Add(new PivotGridFieldSynchronizer(columnViewEditor));
            ModelSynchronizerList.Add(new PivotGridEditorSynchronizer(columnViewEditor));
        }
    }

    public class PivotGridFieldSynchronizer : ModelSynchronizer<DevExpress.XtraPivotGrid.PivotGridControl, IModelListViewOptionsPivotGrid> {
        public PivotGridFieldSynchronizer(PivotGridListEditor component)
            : base(component.PivotGridControl, (IModelListViewOptionsPivotGrid)component.Model) {
        }

        protected override void ApplyModelCore() {
            foreach (var modelColumn in Model.Columns.OfType<IModelColumnOptionsPivotGridField>()) {
                var pivotGridField = Control.Fields[modelColumn.PropertyName];
                ApplyModel(modelColumn.OptionsColumnPivotGridField, pivotGridField, ApplyValues);
            }
        }

        public override void SynchronizeModel() {

        }
    }
    public class PivotGridEditorSynchronizer : ComponentSynchronizer<DevExpress.XtraPivotGrid.PivotGridControl, IModelOptionsPivotGrid> {
        public PivotGridEditorSynchronizer(PivotGridListEditor pivotGridListEditor)
            : base(pivotGridListEditor.PivotGridControl, ((IModelListViewOptionsPivotGrid)pivotGridListEditor.Model).OptionsPivotGrid, false) {
        }

        public override void SynchronizeModel() {

        }
    }

    public class PivotDataFieldRepositoryItemSpinEditSynchronizer : ModelSynchronizer<RepositoryItemSpinEdit, IModelRepositoryItemSpinEdit> {
        public PivotDataFieldRepositoryItemSpinEditSynchronizer(RepositoryItemSpinEdit component, IModelRepositoryItemSpinEdit modelNode)
            : base(component, modelNode) {
        }
        #region Overrides of ModelSynchronizer
        protected override void ApplyModelCore() {
            if (Model.NodeEnabled)
                ApplyModel(Model, Control, ApplyValues);
        }

        public override void SynchronizeModel() {

        }
        #endregion
    }
}