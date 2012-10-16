using System;

namespace Xpand.Persistent.Base.General {
    public interface IDateRange {

        string Name { get; set; }

        DateTime StartPoint { get; set; }
        DateTime EndPoint { get; set; }
    }
}