using System;

namespace eXpand.Persistent.Base.General {
    public class PictureItemEventArgs : EventArgs
    {
        public IPictureItem ItemClicked;

        public PictureItemEventArgs(IPictureItem itemClicked)
        {
            ItemClicked = itemClicked;
        }
    }
}