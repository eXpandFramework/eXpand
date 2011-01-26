using DevExpress.Xpo;
using FeatureCenter.Base;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General;

namespace FeatureCenter.Module.Miscellaneous.Sequence {
    public enum SeqEnum {
        Zero,
        One
    }

    public class SeqOder : OrderBase, ISupportSequenceObject {
        public SeqOder(Session session)
            : base(session) {
        }
        private SequenceCustomer _sequenceCustomer;
        [ProvidedAssociation("SequenceCustomer-SeqOders")]
        public SequenceCustomer SequenceCustomer {
            get {
                return _sequenceCustomer;
            }
            set {
                SetPropertyValue("SequenceCustomer", ref _sequenceCustomer, value);
            }
        }
        protected override void SetCustomer(ICustomer customer) {
            SequenceCustomer = (SequenceCustomer)customer;

        }
        
        protected override ICustomer GetCustomer() {
            return _sequenceCustomer;
        }
        private string _orderId;
        [SequenceProperty]
        public string OrderId {
            get {
                return _orderId;
            }
            set {
                SetPropertyValue("OrderId", ref _orderId, value);
            }
        }
        protected override void OnSaving() {
            base.OnSaving();
            if (Session.IsNewObject(this)) {
                SequenceGenerator.GenerateSequence(this);
            }
        }
        protected override void OnDeleting() {
            base.OnDeleting();
            SequenceGenerator.ReleaseSequence(this);
        }

        long ISupportSequenceObject.Sequence {
            get { return _sequence; }
            set {
                _sequence = value;
                OrderId = ((ISupportSequenceObject)this).Prefix + value;
            }
        }
        private SeqEnum _seqEnum;
        long _sequence;

        public SeqEnum SeqEnum {
            get {
                return _seqEnum;
            }
            set {
                SetPropertyValue("SeqEnum", ref _seqEnum, value);
            }
        }
        string ISupportSequenceObject.Prefix {
            get {
                return SequenceCustomer.SeqEnum.ToString();
            }

        }
    }

    
    public class SequenceCustomer : CustomerBase, ISupportSequenceObject {

        public SequenceCustomer(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
            if (GetHashCode() % 2 == 0)
                SeqEnum = SeqEnum.One;
        }
        protected override void OnDeleting() {
            base.OnDeleting();
            SequenceGenerator.ReleaseSequence(this);
        }
        
        private int _id;
        [SequenceProperty]
        public int Id {
            get {
                return _id;
            }
            set {
                SetPropertyValue("Id", ref _id, value);
            }
        }

        private SeqEnum _seqEnum;

        public SeqEnum SeqEnum {
            get {
                return _seqEnum;
            }
            set {
                SetPropertyValue("SeqEnum", ref _seqEnum, value);
            }
        }
        protected override void OnSaving() {
            base.OnSaving();
            if (Session.IsNewObject(this))
                SequenceGenerator.GenerateSequence(this);
        }


        long ISupportSequenceObject.Sequence {
            get { return Id; }
            set { Id = (int)value; }
        }

        string ISupportSequenceObject.Prefix {
            get {

                return SeqEnum.ToString();
            }


        }


    }
}
