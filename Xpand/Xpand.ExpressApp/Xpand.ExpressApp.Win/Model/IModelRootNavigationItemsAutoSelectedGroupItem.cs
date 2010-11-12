using System.ComponentModel;

namespace Xpand.ExpressApp.Win.Model {
    public interface IModelRootNavigationItemsAutoSelectedGroupItem {
        [Category("eXpand")]
        [Description("Execute the first navigation item when changing the group when navigation style is Navbar")]
        bool AutoSelectFirstItemInGroup { get; set; }
    }
}
