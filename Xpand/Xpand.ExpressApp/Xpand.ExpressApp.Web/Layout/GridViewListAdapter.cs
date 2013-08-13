using System;
using System.Globalization;
using DevExpress.Web.ASPxGridView;

namespace Xpand.ExpressApp.Web.Layout {
    class GridViewListAdapter : ListControlAdapterBase<ASPxGridView> {


        public override string CreateSetBoundsScript(string widthFunc, string heightFunc) {
            if (string.IsNullOrEmpty(Control.ClientInstanceName))
                Control.ClientInstanceName = GenerateClientInstanceName();

            return string.Format(CultureInfo.InvariantCulture, "var control = {0}; control.SetWidth({1}); control.SetHeight({2});",
                 Control.ClientInstanceName, widthFunc, heightFunc);
        }

        private string GenerateClientInstanceName() {
            return "ClientInstance_" + Guid.NewGuid().ToString("N");
        }

    }
}
