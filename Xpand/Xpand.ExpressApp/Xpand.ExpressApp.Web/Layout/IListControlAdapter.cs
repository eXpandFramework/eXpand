using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Xpand.ExpressApp.Web.Layout {
    public interface IListControlAdapter  {
        Control Control { get; set; }
        string CreateSetBoundsScript(string widthFunc, string heightFunc);
    }
}
