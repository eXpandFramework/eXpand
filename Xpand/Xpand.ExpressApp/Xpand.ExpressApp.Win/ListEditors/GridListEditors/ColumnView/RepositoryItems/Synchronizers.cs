using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Repository;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems {
    public class RepositoryItemColumnViewSynchronizer : RepositoryItemSynchronizer<DevExpress.XtraGrid.Views.Base.ColumnView, IModelListView> {
        public RepositoryItemColumnViewSynchronizer(DevExpress.XtraGrid.Views.Base.ColumnView control, IModelListView modelNode)
            : base(control, modelNode) {
        }

        protected override void ApplyModelCore() {
            foreach (var modelColumn in Model.Columns.OfType<IModelMemberViewItemRepositoryItem>()) {
                var gridColumn = Control.Columns[modelColumn.PropertyName];
                if (gridColumn != null) {
                    var modelRepositoryItems = GetRepositoryItems(gridColumn.ColumnEdit, modelColumn);
                    foreach (var modelRepositoryItem in modelRepositoryItems) {
                        ApplyModel(modelRepositoryItem, gridColumn.ColumnEdit, ApplyValues);
                    }
                }
            }
        }

        IEnumerable<IModelRepositoryItem> GetRepositoryItems(RepositoryItem repositoryItem, IModelMemberViewItemRepositoryItem modelMemberViewItem) {
            return modelMemberViewItem.RepositoryItems.Where(item => FindRepository(repositoryItem, item));
        }

        public override void SynchronizeModel() {

        }
    }

    public class RepositoryItemDetailViewSynchronizer : RepositoryItemSynchronizer<DetailView, IModelDetailView> {
        public RepositoryItemDetailViewSynchronizer(DetailView control)
            : base(control, control.Model) {
        }

        protected override void ApplyModelCore() {
            foreach (var viewItem in ViewItems()) {
                var dxPropertyEditor = Control.GetItems<DXPropertyEditor>().FirstOrDefault(editor => editor.Model == viewItem);
                if (dxPropertyEditor != null) {
                    var repositoryItem = dxPropertyEditor.Control.Properties;
                    var modelRepositoryItems = GetRepositoryItems(repositoryItem, viewItem);
                    foreach (var modelRepositoryItem in modelRepositoryItems) {
                        ApplyModel(modelRepositoryItem, repositoryItem, ApplyValues);
                    }
                }
            }
        }

        IEnumerable<IModelMemberViewItemRepositoryItem> ViewItems() {
            return Model.Items.OfType<IModelMemberViewItemRepositoryItem>().Where(item => item.RepositoryItems.Any());
        }

        IEnumerable<IModelRepositoryItem> GetRepositoryItems(RepositoryItem repositoryItem, IModelMemberViewItemRepositoryItem modelMemberViewItem) {
            return modelMemberViewItem.RepositoryItems.Where(item => FindRepository(repositoryItem, item));
        }


        public override void SynchronizeModel() {
        }
    }

    public abstract class RepositoryItemSynchronizer<T, V> : Persistent.Base.ModelAdapter.ModelSynchronizer<T, V> where V : IModelObjectView {
        protected RepositoryItemSynchronizer(T component, V modelNode)
            : base(component, modelNode) {
        }

        protected bool FindRepository(RepositoryItem repositoryItem, IModelRepositoryItem item) {
            if (!item.NodeEnabled) return false;
            var repoType = repositoryItem.GetType();
            var itemTypeName = item.GetType().Name;
            while (itemTypeName != "Model" + repoType.Name) {
                repoType = repoType.BaseType;
                if (repoType == null)
                    return false;
            }
            return true;
        }

    }
}