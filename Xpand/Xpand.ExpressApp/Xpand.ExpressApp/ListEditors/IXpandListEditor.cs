using System;

namespace Xpand.ExpressApp.ListEditors{
    public interface IXpandListEditor{
        void NotifyViewControlsCreated(XpandListView listView);
        event EventHandler<ViewControlCreatedEventArgs> ViewControlsCreated;
    }

    public class ViewControlCreatedEventArgs : EventArgs{
        public ViewControlCreatedEventArgs(bool isRoot){
            IsRoot = isRoot;
        }

        internal bool IsRoot { get; private set; }
    }
}