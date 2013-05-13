using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xpand.ExpressApp.ListEditors {
    public interface IXpandListEditor {
        void NotifyViewControlsCreated(XpandListView listView);
    }
}
