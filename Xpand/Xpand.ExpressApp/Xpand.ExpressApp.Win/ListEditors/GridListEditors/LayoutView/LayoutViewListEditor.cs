using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.MasterDetail;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.LayoutView {
    [ListEditor(typeof(object), false)]
    public class LayoutViewListEditor(IModelListView model) : LayoutViewListEditorBase(model), IColumnViewEditor {
        protected virtual void OnCustomGridViewCreate(CustomGridViewCreateEventArgs e) {
            EventHandler<CustomGridViewCreateEventArgs> handler = CustomGridViewCreate;
            handler?.Invoke(this, e);
        }

        public event EventHandler<CustomGridViewCreateEventArgs> CustomGridViewCreate;
        protected override GridColumnModelSynchronizer CreateGridColumnModelSynchronizer(GridColumnModelSynchronizerParameters parameters) 
            => new CustomGridColumnModelSynchronizer(parameters, this);
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        bool IColumnViewEditor.OverrideViewDesignMode { get; set; }

        protected override void OnColumnCreated(GridColumn column, IModelColumn columnInfo) {
            base.OnColumnCreated(column, columnInfo);
            if (column.ColumnEdit is RepositoryItemMemoExEdit)
                column.ColumnEdit = new RepositoryItemMemoEdit { Name = columnInfo.PropertyName };
        }
        protected override DevExpress.XtraGrid.Views.Base.ColumnView CreateGridViewCore() {
            var gridViewCreatingEventArgs = new CustomGridViewCreateEventArgs(Grid);
            OnCustomGridViewCreate(gridViewCreatingEventArgs);
            return gridViewCreatingEventArgs.Handled
                ? gridViewCreatingEventArgs.GridView
                : new XpandXafLayoutView(this) { OverrideViewDesignMode = ((IColumnViewEditor)this).OverrideViewDesignMode };
        }

        bool ISupportFooter.IsFooterVisible {
            get => false;
            set { }
        }

    }

    public class XpandXafLayoutView : XafLayoutView, IMasterDetailColumnView {

        public XpandXafLayoutView(GridControl gridControl)
            : base(gridControl) {
        }
        public override void Assign(BaseView v, bool copyEvents) {
            var xafGridView = (IMasterDetailColumnView)v;
            ((IMasterDetailColumnView)this).Window = xafGridView.Window;
            ((IMasterDetailColumnView)this).MasterFrame = xafGridView.MasterFrame;
            base.Assign(v, copyEvents);
        }
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        #region Implementation of IMasterDetailColumnView
        public Window Window { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Frame MasterFrame { get; set; }

        int IMasterDetailColumnView.GetRelationIndex(int sourceRowHandle, string levelName) => throw new NotImplementedException();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanFilterGroupSummaryColumns { get; set; }
        #endregion
        public XpandXafLayoutView(LayoutViewListEditor layoutViewListEditor)
            : base(layoutViewListEditor.Grid) {
        }

        protected override bool IsDesignMode => OverrideViewDesignMode || base.IsDesignMode;

        protected override BaseView CreateInstance() => new XpandXafLayoutView(GridControl);

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool OverrideViewDesignMode { get; set; }
    }

    public class CustomGridColumnModelSynchronizer(
        GridColumnModelSynchronizerParameters parameters,
        LayoutViewListEditorBase editor)
        : GridColumnModelSynchronizer(parameters) {
        protected override ModelSynchronizer CreateModelSynchronizer(GridColumn column) {
            ModelSynchronizer baseSynchronizer = base.CreateModelSynchronizer(column);

            var result = new CompositeModelSynchronizer(editor, Model);
            result.Add(baseSynchronizer);

            var customSynchronizables = new List<IModelSynchronizable> {
                new FilterModelSynchronizer(editor, (IModelListView)Model.ParentView),
                new LayoutViewListEditorSynchronizer(editor)
            };

            if (customSynchronizables.Count > 0) {
                result.Add(new SimpleModelSynchronizer(editor, Model, customSynchronizables));
            }

            return result;
        }
        
        public class SimpleModelSynchronizer(
            object control,
            IModelNode model,
            IEnumerable<IModelSynchronizable> synchronization)
            : ModelSynchronizer(control, model) {
            protected override void ApplyModelCore() {
                foreach (var item in synchronization) {
                    item.ApplyModel();
                }
            }


            public override void SynchronizeModel() {
                foreach (var item in synchronization) {
                    item.SynchronizeModel();
                }
            }
        }
        public class CompositeModelSynchronizer(object control, IModelNode model) : ModelSynchronizer(control, model) {
            private readonly List<ModelSynchronizer> _synchronizes = new();

            public void Add(ModelSynchronizer synchronizer) {
                if (synchronizer != null) {
                    _synchronizes.Add(synchronizer);
                }
            }

            protected override void ApplyModelCore() {
                foreach (var synchronizer in _synchronizes) {
                    synchronizer.ApplyModel();
                }
            }

            public override void SynchronizeModel() {
                foreach (var synchronizer in _synchronizes) {
                    synchronizer.SynchronizeModel();
                }
            }

            public override void Dispose() {
                base.Dispose();
                foreach (var synchronizer in _synchronizes) {
                    synchronizer.Dispose();
                }
                _synchronizes.Clear();
            }
        }
    }
}
