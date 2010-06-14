namespace eXpand.Persistent.Base.General {
    public class RequestTextPictureItemEventArgs : PictureItemEventArgs
    {
        public RequestTextPictureItemEventArgs(IPictureItem itemClicked)
            : base(itemClicked)
        {
        }

        public string Text { get; set; }
    }
}