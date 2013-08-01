using System.ComponentModel;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.XtraGrid.Columns;
using Xpand.ExpressApp.Model;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems;
using Xpand.Persistent.Base.General.Model.Options;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.LayoutView.Model {
    public class LayoutViewLstEditorDynamicModelSynchronizer : ModelListSynchronizer {
        public LayoutViewLstEditorDynamicModelSynchronizer(LayoutViewListEditor columnViewEditor)
            : base(columnViewEditor, columnViewEditor.Model) {

            ModelSynchronizerList.Add(new LayoutViewLayoutStoreSynchronizer(columnViewEditor));
            ModelSynchronizerList.Add(new LayoutViewListEditorSynchronizer(columnViewEditor, columnViewEditor.Model));
            ModelSynchronizerList.Add(new LayoutViewOptionsSynchronizer(columnViewEditor));
            ModelSynchronizerList.Add(new LayoutColumnOptionsSynchroniser(columnViewEditor));
            ModelSynchronizerList.Add(new RepositoryItemColumnViewSynchronizer(columnViewEditor.GridView, columnViewEditor.Model));

        }
    }

    public class LayoutViewLayoutStoreSynchronizer : ColumnViewEditorLayoutStoreSynchronizer {
        public LayoutViewLayoutStoreSynchronizer(LayoutViewListEditor control)
            : base(control, control.Model.OptionsLayoutView.DesignLayoutView) {
        }
    }
    public class LayoutViewOptionsSynchronizer : ComponentSynchronizer<XafLayoutView, IModelOptionsLayoutView> {
        public LayoutViewOptionsSynchronizer(LayoutViewListEditor layoutViewListEditor)
            : base(layoutViewListEditor.GridView, layoutViewListEditor.Model.OptionsLayoutView, ((IColumnViewEditor)layoutViewListEditor).OverrideViewDesignMode) {
        }
        protected override object GetSynchronizeValuesNodeValue(ModelNode modelNode, ModelValueInfo valueInfo, PropertyDescriptor propertyDescriptor, bool isNullableType, object component) {
            var overrideViewDesignMode = OverrideViewDesignMode;
            var synchronizeValuesNodeValue = base.GetSynchronizeValuesNodeValue(modelNode, valueInfo, propertyDescriptor, !overrideViewDesignMode && isNullableType, component);
            if (overrideViewDesignMode)
                return PropertyDefaultValue(synchronizeValuesNodeValue, modelNode, propertyDescriptor, valueInfo, component);
            return synchronizeValuesNodeValue;
        }
    }

    public class LayoutColumnOptionsSynchroniser : ColumnViewEditorColumnOptionsSynchronizer<LayoutViewListEditor, IModelListViewOptionsLayoutView, IModelColumnOptionsLayoutView> {
        public LayoutColumnOptionsSynchroniser(LayoutViewListEditor control)
            : base(control, control.Model) {
        }

        protected override void ApplyModelCore() {
            base.ApplyModelCore();
            foreach (GridColumn column in Control.GridView.Columns) {
                column.OptionsColumn.AllowEdit = Control.Model.AllowEdit;
            }
        }
        protected override object GetSynchronizeValuesNodeValue(ModelNode modelNode, ModelValueInfo valueInfo, PropertyDescriptor propertyDescriptor, bool isNullableType, object component) {
            var overrideViewDesignMode = ((IColumnViewEditor)Control).OverrideViewDesignMode;
            var synchronizeValuesNodeValue = base.GetSynchronizeValuesNodeValue(modelNode, valueInfo, propertyDescriptor, !overrideViewDesignMode && isNullableType, component);
            if (overrideViewDesignMode)
                return PropertyDefaultValue(synchronizeValuesNodeValue, modelNode, propertyDescriptor, valueInfo, component);
            return synchronizeValuesNodeValue;
        }

        protected override DevExpress.XtraGrid.Views.Base.ColumnView GetColumnView() {
            return Control.GridView;
        }

        protected override IModelColumnViewColumnOptions GetColumnOptions(IModelColumnOptionsLayoutView modelColumnOptionsView) {
            return modelColumnOptionsView.OptionsColumnLayoutView;
        }
    }
}
