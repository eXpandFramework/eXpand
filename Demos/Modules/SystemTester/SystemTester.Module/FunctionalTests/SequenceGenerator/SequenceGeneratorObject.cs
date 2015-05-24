using System;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;

namespace SystemTester.Module.FunctionalTests.SequenceGenerator{
    [DefaultClassOptions]
    public class SequenceGeneratorObject : BaseObject, ISupportSequenceObject{
        private string _prefix;
        private long _sequence;

        public SequenceGeneratorObject(Session session) : base(session){
        }

        public long Sequence{
            get { return _sequence; }
            set { SetPropertyValue("Sequence", ref _sequence, value); }
        }


        public string Prefix{
            get { return _prefix; }
            set { SetPropertyValue("Prefix", ref _prefix, value); }
        }

        protected override void OnSaving(){
            base.OnSaving();
            Xpand.Persistent.Base.General.SequenceGenerator.GenerateSequence(this);
        }

        [Association("SequenceGeneratorObject-SequenceGeneratorNestedObjects"), Aggregated]
        public XPCollection<SequenceGeneratorNestedObject> SequenceGeneratorNestedObjects{
            get { return GetCollection<SequenceGeneratorNestedObject>("SequenceGeneratorNestedObjects"); }
        }

        [Action]
        public void UpdateSequence(){
            Guid guid = Oid;
            ClassInfo.KeyProperty.SetValue(this, Guid.Empty);
            Xpand.Persistent.Base.General.SequenceGenerator.GenerateSequence(this);
            ClassInfo.KeyProperty.SetValue(this, guid);
        }
    }

    public class SequenceGeneratorNestedObject : BaseObject, ISupportSequenceObject {
        private SequenceGeneratorObject _sequenceGeneratorObject;

        public SequenceGeneratorNestedObject(Session session) : base(session){
        }


        [Association("SequenceGeneratorObject-SequenceGeneratorNestedObjects")]
        public SequenceGeneratorObject SequenceGeneratorObject{
            get { return _sequenceGeneratorObject; }
            set { SetPropertyValue("SequenceGeneratorObject", ref _sequenceGeneratorObject, value); }
        }

        public long Sequence { get; set; }

        protected override void OnSaving(){
            base.OnSaving();
            Xpand.Persistent.Base.General.SequenceGenerator.GenerateSequence(this);
        }

        public string Prefix{
            get { return SequenceGeneratorObject.Sequence+". "; }
        }
    }
}