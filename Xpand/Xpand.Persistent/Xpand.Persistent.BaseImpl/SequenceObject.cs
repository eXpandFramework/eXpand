using System.Collections.Generic;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.General;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl {
    public class SequenceObject : XpandCustomObject, ISequenceObject {
        private string _typeName;
        private long _nextSequence;
        public SequenceObject(Session session)
            : base(session) {
        }

        public string TypeName{
            get { return _typeName; }
            set { SetPropertyValue("TypeName", ref _typeName, value); }
        }

        public long NextSequence {
            get { return _nextSequence; }
            set { SetPropertyValue("NextSequence", ref _nextSequence, value); }
        }

        IList<ISequenceReleasedObject> ISequenceObject.SequenceReleasedObjects {
            get { return new ListConverter<ISequenceReleasedObject, SequenceReleasedObject>(XpandReleasedSequences); }
        }

        [Association("SequenceObject-XpandReleasedSequences")]
        [Aggregated]
        public XPCollection<SequenceReleasedObject> XpandReleasedSequences {
            get {
                return GetCollection<SequenceReleasedObject>("XpandReleasedSequences");
            }
        }
    }


    [Appearance("Hide_NewAction_for_SequenceReleasedObject", AppearanceItemType.Action, "1=1", TargetItems = "New", Visibility = ViewItemVisibility.Hide)]
    public class SequenceReleasedObject : XpandBaseCustomObject, ISequenceReleasedObject {
        public SequenceReleasedObject(Session session)
            : base(session) {
        }
        private SequenceObject _sequenceObject;
        [VisibleInListView(false)]
        [Association("SequenceObject-XpandReleasedSequences")]
        public SequenceObject SequenceObject {
            get {
                return _sequenceObject;
            }
            set {
                SetPropertyValue("SequenceObject", ref _sequenceObject, value);
            }
        }
        private long _sequence;

        ISequenceObject ISequenceReleasedObject.SequenceObject {
            get { return SequenceObject; }
            set { SequenceObject = value as SequenceObject; }
        }

        public long Sequence {
            get {
                return _sequence;
            }
            set {
                SetPropertyValue("Sequence", ref _sequence, value);
            }
        }
    }

}