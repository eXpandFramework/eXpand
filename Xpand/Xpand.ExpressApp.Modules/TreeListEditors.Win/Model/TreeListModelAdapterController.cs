using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.TreeListEditors.Win;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using Xpand.ExpressApp.Core;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.TreeListEditors.Win.Model {
    public class TreeListModelAdapterController : ModelAdapterController, IModelExtender {
        TreeListEditor _treeListEditor;

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (_treeListEditor != null)
                _treeListEditor.CreateCustomModelSynchronizer -= GridListEditorOnCreateCustomModelSynchronizer;
        }

        void GridListEditorOnCreateCustomModelSynchronizer(object sender, CreateCustomModelSynchronizerEventArgs e) {
            CreateCustomModelSynchronizerHelper.Assign(e, new TreeListEditorDynamicModelSynchronizer(_treeListEditor));
        }

        protected override void OnActivated() {
            base.OnActivated();
            var listView = View as ListView;
            if (listView != null && listView.Editor != null && listView.Editor is TreeListEditor) {
                _treeListEditor = (TreeListEditor)listView.Editor;
                _treeListEditor.CreateCustomModelSynchronizer += GridListEditorOnCreateCustomModelSynchronizer;
            }
        }
        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelListView, IModelListViewOptionsTreeList>();
            extenders.Add<IModelColumn, IModelColumnOptionsTreeListView>();

            var builder = new InterfaceBuilder(extenders);

            var assembly = builder.Build(CreateBuilderData(), GetPath(typeof(TreeList).Name));

            builder.ExtendInteface<IModelOptionsTreeList, TreeList>(assembly);
            builder.ExtendInteface<IModelOptionsColumnTreeListView, TreeListColumn>(assembly);
        }

        IEnumerable<InterfaceBuilderData> CreateBuilderData() {
            yield return new InterfaceBuilderData(typeof(TreeList)) {
                Act = info => info.DXFilter()
            };
            yield return new InterfaceBuilderData(typeof(TreeListColumn)) {
                Act = info => info.DXFilter()
            };
        }
    }
}
