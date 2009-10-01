using System;
using eXpand.Persistent.Base.General;

namespace eXpand.Persistent.Base.General{
    [Serializable]
    public class DateRange : IDateRange{
        #region IDateRange Members
        public DateTime? FirstDay { get; set; }
        public DateTime? LastDay { get; set; }

        public DateTime? LastMinute{
            get { return (IsEmpty ? (DateTime?) null : LastDay.Value.AddDays(1).AddSeconds(-1)); }
        }

        public bool IsSingleDay{
            get { return (!IsEmpty && FirstDay.Equals(LastDay)); }
        }

        public bool IsEmpty{
            get { return !FirstDay.HasValue || !LastDay.HasValue; }
        }

        public string Name{
            get{
                if (IsEmpty){
                    return string.Empty;
                }
                if (IsSingleDay) {
                    return FirstDay.Value.ToShortDateString();
                }
                return string.Format("{0} - {1}", FirstDay.Value.ToShortDateString(), LastDay.Value.ToShortDateString());
            }
        }

        public int NumberOfDays{
            get { return IsEmpty ? 0 : (int) Math.Ceiling(LastDay.Value.Subtract(FirstDay.Value).TotalDays) + 1; }
        }
        #endregion
    }
}