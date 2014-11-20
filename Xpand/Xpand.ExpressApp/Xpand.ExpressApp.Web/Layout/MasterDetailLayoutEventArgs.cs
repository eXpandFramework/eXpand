using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xpand.ExpressApp.Web.Layout {
    public class MasterDetailLayoutEventArgs : EventArgs {
        public DevExpress.ExpressApp.Editors.ViewItem MasterViewItem { get; internal set; }

        public DevExpress.ExpressApp.Editors.ViewItem DetailViewItem { get; internal set; }

        public DevExpress.Web.ASPxSplitter SplitterControl { get; internal set; }
    }
}
