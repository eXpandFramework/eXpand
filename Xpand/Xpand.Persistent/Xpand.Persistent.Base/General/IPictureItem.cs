using System.Drawing;

namespace Xpand.Persistent.Base.General {
    public interface IPictureItem {
        string ID { get; }
        Image Image { get; }
        string ImagePath { get; }
    }
}