using System.Drawing;

namespace eXpand.Persistent.Base.General {
    public interface INCarouselItem : IPictureItem {
        
    }

    public interface IThumbNailItem:IPictureItem {
    }

    public interface IPictureItem {
        string ID { get; }
        Image Image { get; }
        string ImagePath { get; }
    }
}