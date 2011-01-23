using System.Collections.Generic;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.General;

namespace Xpand.Persistent.BaseImpl {

    public class SequenceObject : XPBaseObject, ISequenceObject {
        private string typeName;
        private long nextSequence;
        public SequenceObject(Session session)
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

        IList<ISequenceReleasedObject> ISequenceObject.SequenceReleasedObjects {
            get { return new ListConverter<ISequenceReleasedObject,SequenceReleasedObject>(XpandReleasedSequences); }
        }

        [Association("SequenceObject-XpandReleasedSequences")]
        [Aggregated]
        public XPCollection<SequenceReleasedObject> XpandReleasedSequences {
            get {
                return GetCollection<SequenceReleasedObject>("XpandReleasedSequences");
            }
        }
    }

    [DefaultClassOptions]
    [Appearance("Hide_NewAction_for_SequenceReleasedObject",AppearanceItemType.Action, "1=1",TargetItems = "New",Visibility = ViewItemVisibility.Hide)]
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