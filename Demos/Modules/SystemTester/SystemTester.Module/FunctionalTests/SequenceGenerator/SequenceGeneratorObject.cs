using System;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;

namespace SystemTester.Module.FunctionalTests.SequenceGenerator {
    [DefaultClassOptions]
    public class SequenceGeneratorObject:BaseObject,ISupportSequenceObject {
        public SequenceGeneratorObject(Session session) : base(session){
        }

        protected override void OnSaving() {
            base.OnSaving();
            Xpand.Persistent.Base.General.SequenceGenerator.GenerateSequence(this);
        }
        [Action]
        public void UpdateSequence(){
            var guid = Oid;
            ClassInfo.KeyProperty.SetValue(this,Guid.Empty);
            Xpand.Persistent.Base.General.SequenceGenerator.GenerateSequence(this);
            ClassInfo.KeyProperty.SetValue(this, guid);
        }
        public long Sequence { get; set; }

        public string Prefix { get; set; }
    }
}
