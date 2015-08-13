using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.DashboardWin;

namespace Xpand.ExpressApp.XtraDashboard.Win.Controllers
{
    public class DashboardDesignerOpeningEventArgs : EventArgs
    {
        public DashboardDesignerOpeningEventArgs(DashboardDesigner designer)
        {
            Designer = designer;
        }

        public DashboardDesigner Designer { get; private set; }
    }
}
