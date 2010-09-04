using NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition;

namespace Xpand.ExpressApp.Win.Core
{
    public class DynamicDouplicateTypesMapper : Xpand.ExpressApp.Core.DynamicModel.DynamicDouplicateTypesMapper
    {
        public DynamicDouplicateTypesMapper() {
            Add(typeof(NewItemRowPosition), typeof(eXpandNewItemRowPosition));
        }
    }

    public enum eXpandNewItemRowPosition {
        None, Top, Bottom
    }
}
