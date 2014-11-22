using System;
using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Xpo;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design {
    public abstract class ColumnViewPropertyEditor<TColumnViewEditor> : UITypeEditor where TColumnViewEditor : WinColumnsListEditor {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
            var modelListView = GetListView(context);
            if (modelListView != null)
                ShowEditor(modelListView);
            return Guid.NewGuid().ToString();
        }

        IModelListView GetListView(ITypeDescriptorContext context) {
            if (context.Instance != null) {
                var modelNode = ((ModelNode)context.Instance);
                while (!(modelNode is IModelListView)) {
                    modelNode = modelNode.Parent;
                }
                var listView = modelNode as IModelListView;
                return listView.ModelClass != null ? listView : null;
            }
            return null;
        }

        void ShowEditor(IModelListView listView) {
            using (var editor = GetViewDesignerForm(listView)) {
                var gridListEditor = CreateListEditor(listView);
                var mainView = GetColumnView(gridListEditor);
                editor.InitEditingObject(mainView);
                editor.ShowDialog();
                gridListEditor.SaveModel();
            }
        }

        protected virtual BaseView GetColumnView(TColumnViewEditor gridListEditor) {
            var mainView = gridListEditor.Grid.MainView;
            return mainView;
        }



        protected abstract ColumnViewDesignerForm GetViewDesignerForm(IModelListView listView);

        public TColumnViewEditor CreateListEditor(IModelListView listView) {
            var gridListEditor = GetGridDesignerEditor(listView);
            ((IColumnViewEditor) gridListEditor).OverrideViewDesignMode = true;
            var targetType = listView.ModelClass.TypeInfo.Type;
            Setup(targetType, gridListEditor);
            gridListEditor.Grid.CreateControl();
            gridListEditor.DataSource = CriteriaPropertyEditorHelper.CreateFilterControlDataSource(targetType, null);
            var form = new System.Windows.Forms.Form();
            form.Controls.Add(gridListEditor.Grid);
            return gridListEditor;
        }

        protected abstract TColumnViewEditor GetGridDesignerEditor(IModelListView listView);

        void Setup(Type targetType, TColumnViewEditor gridListEditor) {
            XpoTypesInfoHelper.ForceInitialize();
            var xpObjectSpace = new XPObjectSpace(XafTypesInfo.Instance, ((XpoTypeInfoSource)XafTypesInfo.PersistentEntityStore), () => null);
            var collectionSource = new CollectionSource(xpObjectSpace, targetType);
            gridListEditor.Setup(collectionSource, new EditorsXafApplication());
            gridListEditor.ColumnCreated += (sender, args) => {
                var column = args.Column;
                if (column.ColumnEdit != null) {
                    column.ColumnEdit.Name = column.Name;
                    column.ColumnEdit.Site = new MySite(gridListEditor.Grid.MainView, column);
                }
            };
            gridListEditor.CreateControls();
        }
        class MySite : ISite {
            readonly IServiceProvider _sp;
            readonly GridColumn _comp;

            public MySite(IServiceProvider sp, GridColumn comp) {
                _sp = sp;
                _comp = comp;
            }

            IComponent ISite.Component {
                get { return _comp; }
            }

            IContainer ISite.Container {
                get { return _sp.GetService(typeof(IContainer)) as IContainer; }
            }

            bool ISite.DesignMode {
                get { return false; }
            }

            string ISite.Name {
                get { return _comp.Name; }
                set { }
            }

            object IServiceProvider.GetService(Type t) {
                return (_sp != null) ? _sp.GetService(t) : null;
            }
        }

        class EditorsXafApplication : XafApplication {
            protected override LayoutManager CreateLayoutManagerCore(bool simple) {
                throw new NotImplementedException();
            }
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
            return UITypeEditorEditStyle.Modal;
        }
    }
}