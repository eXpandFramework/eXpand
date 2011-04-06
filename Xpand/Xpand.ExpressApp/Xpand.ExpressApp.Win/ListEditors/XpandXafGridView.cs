using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Views.Base;

namespace Xpand.ExpressApp.Win.ListEditors {
    public class XpandXafGridView : XafGridView {
        readonly GridListEditor _gridListEditor;
        private static readonly object instanceCreated = new object();


        public XpandXafGridView() {
        }
        public XpandXafGridView(GridListEditor gridListEditor) {
            _gridListEditor = gridListEditor;
        }

        protected virtual void OnInstanceCreated(GridViewInstanceCreatedArgs e) {
            var handler = (EventHandler<GridViewInstanceCreatedArgs>)Events[instanceCreated];
            if (handler != null) handler(this, e);
        }

        [Description("Provides the ability to customize cell merging behavior."), Category("Merge")]
        public event EventHandler<GridViewInstanceCreatedArgs> GridViewInstanceCreated {
            add { Events.AddHandler(instanceCreated, value); }
            remove { Events.RemoveHandler(instanceCreated, value); }
        }


        public Window Window { get; set; }

        public Frame MasterFrame { get; set; }




        protected override BaseView CreateInstance() {
            var view = new XpandXafGridView(_gridListEditor);
            view.SetGridControl(GridControl);
            OnInstanceCreated(new GridViewInstanceCreatedArgs(view));
            return view;
        }
        public override void Assign(BaseView v, bool copyEvents) {
            var xafGridView = ((XpandXafGridView)v);
            Window = xafGridView.Window;
            MasterFrame = xafGridView.MasterFrame;
            Events.AddHandler(instanceCreated, xafGridView.Events[instanceCreated]);
            base.Assign(v, copyEvents);
        }
        protected override void AssignColumns(ColumnView cv, bool synchronize) {
            if (_gridListEditor == null) {
                base.AssignColumns(cv, synchronize);
                return;
            }
            if (synchronize) {
                base.AssignColumns(cv, true);
            } else {
                Columns.Clear();
                ////var columnsListEditorModelSynchronizer = new ColumnsListEditorModelSynchronizer(_gridListEditor, _gridListEditor.Model);
                ////columnsListEditorModelSynchronizer.ApplyModel();
                var gridColumns = _gridListEditor.GridView.Columns.OfType<XafGridColumn>();
                foreach (var column in gridColumns) {
                    var xpandXafGridColumn = new XpandXafGridColumn(column.TypeInfo, _gridListEditor);
                    xpandXafGridColumn.ApplyModel(column.Model);
                    Columns.Add(xpandXafGridColumn);
                    xpandXafGridColumn.Assign(column);
                }
            }
        }
    }
}