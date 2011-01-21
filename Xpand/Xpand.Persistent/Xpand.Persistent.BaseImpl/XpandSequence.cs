using DevExpress.Xpo;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.General;

namespace Xpand.Persistent.BaseImpl {

    public class XpandSequence : XPBaseObject, ISequenceObject {
        private string typeName;
        private long nextSequence;
        public XpandSequence(Session session)
            : base(session) {
        }
        [Key]
        [Size(1024)]
        public string TypeName {
            get { return typeName; }
            set { SetPropertyValue("TypeName", ref typeName, value); }
        }
        public long NextSequence {
            get { return nextSequence; }
            set { SetPropertyValue("NextSequence", ref nextSequence, value); }
        }
        [Association("XpandSequence-XpandReleasedSequences")]
        [Aggregated]
        public XPCollection<SequenceReleased> XpandReleasedSequences {
            get {
                return GetCollection<SequenceReleased>("XpandReleasedSequences");
            }
        }
    }


    public class SequenceReleased : XpandBaseCustomObject, ISequenceReleased {
        public SequenceReleased(Session session)
            : base(session) {
        }
        private XpandSequence _xpandSequence;
        [Association("XpandSequence-XpandReleasedSequences")]
        public XpandSequence XpandSequence {
            get {
                return _xpandSequence;
            }
            set {
                SetPropertyValue("XpandSequence", ref _xpandSequence, value);
            }
        }
        private int _sequence;

        ISequenceObject ISequenceReleased.SequenceObject {
            get { return XpandSequence; }
            set { XpandSequence = value as XpandSequence; }
        }

        public int Sequence {
            get {
                return _sequence;
            }
            set {
                SetPropertyValue("Sequence", ref _sequence, value);
            }
        }
    }

}