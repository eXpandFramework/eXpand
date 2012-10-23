using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.Model {
    public class GridViewModelAdapterController : GridViewModelAdapterControllerBase {
        GridListEditor _gridListEditor;

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (_gridListEditor != null)
                _gridListEditor.CreateCustomModelSynchronizer -= GridListEditorOnCreateCustomModelSynchronizer;
        }
        protected override void OnActivated() {
            base.OnActivated();
            var listView = View as ListView;
            if (listView != null && listView.Editor != null && listView.Editor.GetType() == typeof(GridListEditor)) {
                _gridListEditor = (GridListEditor)listView.Editor;
                _gridListEditor.CreateCustomModelSynchronizer += GridListEditorOnCreateCustomModelSynchronizer;
            }
        }
        void GridListEditorOnCreateCustomModelSynchronizer(object sender, CreateCustomModelSynchronizerEventArgs createCustomModelSynchronizerEventArgs) {
            CreateCustomModelSynchronizerHelper.Assign(createCustomModelSynchronizerEventArgs, new GridLstEditorDynamicModelSynchronizer(_gridListEditor));
        }
        protected override void ExtendInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelListView, IModelListViewOptionsGridView>();
            extenders.Add<IModelColumn, IModelColumnOptionsGridView>();

            var builder = new InterfaceBuilder(extenders);
            var assembly = BuildAssembly(builder, typeof(XafGridView), typeof(XafGridColumn));


            builder.ExtendInteface<IModelOptionsGridView, XafGridView>(assembly);
            builder.ExtendInteface<IModelOptionsColumnGridView, XafGridColumn>(assembly);
        }

    }

}