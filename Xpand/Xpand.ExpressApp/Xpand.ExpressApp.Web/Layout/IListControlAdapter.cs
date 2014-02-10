using System.Web.UI;

namespace Xpand.ExpressApp.Web.Layout {
    public interface IListControlAdapter  {
        Control Control { get; set; }
        string CreateSetBoundsScript(string widthFunc, string heightFunc);
    }
}
