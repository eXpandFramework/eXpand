using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;

namespace SystemTester.Module.FunctionalTests.SequenceGenerator{
    [DomainComponent]
    [NavigationItem("SequenceGenerator")]
    public interface ISequenceGeneratorObject {
        [SequenceGenerator("DCSequence",true)]
         long Sequence { get; set; }     
    }

   

    [NavigationItem("SequenceGenerator")]
    public class SequenceGeneratorObject : BaseObject{
        
        private long _sequence;

        public SequenceGeneratorObject(Session session) : base(session){
        }

        string _name;

        public string Name{
            get{ return _name; }
            set{ SetPropertyValue(nameof(Name), ref _name, value); }
        }

        [EditorAlias(EditorAliases.ReleasedSequence)]
        public long Sequence{
            get { return _sequence; }
            set { SetPropertyValue("Sequence", ref _sequence, value); }
        }

        protected override void OnDeleting(){
            base.OnDeleting();
            Xpand.Persistent.Base.General.SequenceGenerator.ReleaseSequence(Session, "SequenceA",Sequence);
        }

        protected override void OnSaving(){
            base.OnSaving();
            Xpand.Persistent.Base.General.SequenceGenerator.GenerateSequence(this, "SequenceA",l => Sequence=l);
        }

        [Association("SequenceGeneratorObject-SequenceGeneratorNestedObjects"), DevExpress.Xpo.Aggregated]
        public XPCollection<SequenceGeneratorNestedObject> SequenceGeneratorNestedObjects => GetCollection<SequenceGeneratorNestedObject>("SequenceGeneratorNestedObjects");

        [Action]
        public void UpdateSequence(){
            Xpand.Persistent.Base.General.SequenceGenerator.GenerateSequence(Session , "SequenceA",l => Sequence=l);
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

        public string Prefix => SequenceGeneratorObject.Sequence+". ";
    }
}