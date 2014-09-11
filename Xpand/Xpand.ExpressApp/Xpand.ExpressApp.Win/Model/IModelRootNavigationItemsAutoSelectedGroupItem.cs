using System.ComponentModel;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.Win.Model {
    public interface IModelRootNavigationItemsAutoSelectedGroupItem {
        [Category(AttributeCategoryNameProvider.Xpand)]
        [Description("Execute the first navigation item when changing the group when navigation style is Navbar")]
        bool AutoSelectFirstItemInGroup { get; set; }
    }
}
