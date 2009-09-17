using System.Globalization;

namespace eXpand.Persistent.Base.General{
    public interface ILocalizable {
        string CultureCode { get; set; }
        CultureInfo Culture { get; }
    }
}