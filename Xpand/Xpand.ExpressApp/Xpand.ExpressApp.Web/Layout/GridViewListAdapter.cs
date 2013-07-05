using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using DevExpress.Web.ASPxGridView;

namespace Xpand.ExpressApp.Web.Layout {
    class GridViewListAdapter : ListControlAdapterBase<ASPxGridView> {


        public override string CreateSetBoundsScript(string widthFunc, string heightFunc) {
            return string.Format(CultureInfo.InvariantCulture, "var control = ASPxClientControl.GetControlCollection().GetByName('{0}'); control.SetWidth({1}); control.SetHeight({2});",
                 Control.ClientID, widthFunc, heightFunc);
        }

    }
}
