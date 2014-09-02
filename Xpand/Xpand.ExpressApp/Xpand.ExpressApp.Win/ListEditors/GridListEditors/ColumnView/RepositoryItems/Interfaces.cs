using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems {
    [ModelAbstractClass]
    public interface IModelMemberViewItemRepositoryItem : IModelMemberViewItem {
        IModelRepositoryItems RepositoryItems { get; }
    }

    public interface IModelRepositoryItems : IModelNode, IModelList<IModelRepositoryItem> {
    }

    [DomainLogic(typeof(IModelRepositoryItemCheckEdit))]
    public class ModelRepositoryItemDomainLogic{
        public static string Get_Id(IModelRepositoryItemCheckEdit modelRepositoryItem) {
            return new FastModelEditorHelper().GetNodeDisplayName(modelRepositoryItem.GetType());
        }
    }
    [ModelDisplayName("Item")]
    [ModelAbstractClass]
    public interface IModelRepositoryItem : IModelModelAdapter {

    }
}