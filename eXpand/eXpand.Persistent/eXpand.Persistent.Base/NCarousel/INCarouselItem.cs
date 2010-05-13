using System.Drawing;

namespace eXpand.Persistent.Base.NCarousel {
    public interface INCarouselItem
    {
        string ID { get; }
        Image Image { get; }
    }
}