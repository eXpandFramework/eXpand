using NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition;

namespace eXpand.ExpressApp.Win.Core
{
    public class DynamicDouplicateTypesMapper : ExpressApp.Core.DynamicModel.DynamicDouplicateTypesMapper
    {
        public DynamicDouplicateTypesMapper() {
            Add(typeof(NewItemRowPosition), typeof(eXpandNewItemRowPosition));
        }
    }

    public enum eXpandNewItemRowPosition {
        None, Top, Bottom
    }
}
