using System;

namespace eXpand.Persistent.Base.General{
    public interface IDateRange{
        DateTime? FirstDay { get; set; }
        DateTime? LastDay { get; set; }
        DateTime? LastMinute { get; }
        bool IsSingleDay { get; }
        bool IsEmpty { get; }
        string Name { get; }
        int NumberOfDays { get; }
    }
}