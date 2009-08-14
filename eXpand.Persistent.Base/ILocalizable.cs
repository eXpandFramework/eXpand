using System.Globalization;

namespace eXpand.Persistent.Base.Interfaces {
    public interface ILocalizable {
        string CultureCode { get; set; }
        CultureInfo Culture { get; }
    }
}