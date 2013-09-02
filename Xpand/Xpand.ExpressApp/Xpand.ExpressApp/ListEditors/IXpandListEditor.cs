using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xpand.ExpressApp.ListEditors {
    public interface IXpandListEditor {

        void NotifyViewControlsCreated(XpandListView listView);
        event EventHandler<ViewControlCreatedEventArgs> ViewControlsCreated;
    }

    public class ViewControlCreatedEventArgs : EventArgs
    {
        public ViewControlCreatedEventArgs(bool isRoot)
        {
            this.IsRoot = isRoot;
        }

        internal bool IsRoot { get; private set; }
    }
}
