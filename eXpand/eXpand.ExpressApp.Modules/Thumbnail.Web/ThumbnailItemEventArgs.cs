using System;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.Thumbnail.Web {
    public class ThumbnailItemEventArgs : EventArgs {
        public IThumbNailItem ItemClicked;

        public ThumbnailItemEventArgs(IThumbNailItem itemClicked)
        {
            ItemClicked = itemClicked;
        }
    }
}