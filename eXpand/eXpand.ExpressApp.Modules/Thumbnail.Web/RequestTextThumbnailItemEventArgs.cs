using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.Thumbnail.Web {
    public class RequestTextThumbnailItemEventArgs:ThumbnailItemEventArgs {
        public RequestTextThumbnailItemEventArgs(IThumbNailItem itemClicked)
            : base(itemClicked)
        {
        }

        public string Text { get; set; }
    }
}