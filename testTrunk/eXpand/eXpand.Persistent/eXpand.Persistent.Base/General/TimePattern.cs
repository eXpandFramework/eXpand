using System;
using System.Collections.Generic;
using System.Linq;

namespace eXpand.Persistent.Base.General{
    [Serializable]
    public class TimePattern : List<TimeRange>{
        public TimePattern() {}

        public TimePattern(IList<TimeRange> timeRanges) : base(timeRanges) {}

        public TimeSpan Duration{
            get { return new TimeSpan(this.Select(pattern => pattern.Duration.Ticks).Sum()); }
        }

        public double TotalHours{
            get { return this.Sum(pattern => pattern.Duration.TotalHours); }
        }

        public string Caption{
            get{
                if (IsEmpty) return string.Empty;
                string caption = string.Empty;
                foreach (TimeRange timePattern in this){
                    caption = caption == string.Empty 
                                  ? timePattern.Caption
                                  : timePattern.IsEmpty 
                                        ? caption
                                        : string.Format("{0} & {1}", caption ,timePattern.Caption);
                }
                return caption;
            }
        }

        public bool IsEmpty{
            get { return Count == 0; }
        }

        public bool IsComposite{
            get { return Count > 1; }
        }

        public TimeSpan StartTime{
            get { return !IsEmpty ? this.Min(pattern => pattern.StartTime) : TimeSpan.Zero; }
        }

        public TimeSpan EndTime{
            get { return !IsEmpty ? StartTime.Add(Duration) : TimeSpan.Zero; }
        }

        public override string ToString(){
            return Caption;
        }
    }
}