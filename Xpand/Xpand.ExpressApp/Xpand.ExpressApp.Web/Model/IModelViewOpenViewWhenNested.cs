using System.ComponentModel;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Web.Model {
    public enum OpenViewWhenNestedStrategy {
        Default,
        InMainWindow
    }
    [ModelAbstractClass]
    public interface IModelListViewOpenViewWhenNested  {
        [Category("eXpand")]
        [Description("Works only with XpandShowViewStragey or derive from XpandWebApplication")]
        OpenViewWhenNestedStrategy OpenViewWhenNestedStrategy { get; set; }
    }
}
