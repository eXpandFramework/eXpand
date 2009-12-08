using System;

namespace eXpand.Persistent.Base.General{
    [Serializable]
    public class TimeRange {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan Duration {
            get {
                return EndTime.Subtract(StartTime);
            }
        }
        public bool IsEmpty {
            get {
                return (StartTime == TimeSpan.Zero && EndTime == TimeSpan.Zero);
            }
        }
        public string Caption {
            get {
                if (StartTime == TimeSpan.Zero && EndTime == TimeSpan.Zero) { return "(empty)"; }
                return string.Format("{0:D2}:{1:D2} - {2:D2}:{3:D2}", StartTime.Hours, StartTime.Minutes, EndTime.Hours, EndTime.Minutes);
            }
        }
    }
}