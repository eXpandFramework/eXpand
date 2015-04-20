using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;

namespace SystemTester.Module.FunctionalTests.SqlLite {
    [DefaultClassOptions]
    public class SqliteObject : BaseObject, ISupportSequenceObject {
        private readonly string _prefix=null;

        public SqliteObject(Session session) : base(session){
        }
        protected override void OnSaving() {
            base.OnSaving();
            if (Session.IsNewObject(this)){
                Xpand.Persistent.Base.General.SequenceGenerator.ThrowProviderSupportedException = false;
                Xpand.Persistent.Base.General.SequenceGenerator.GenerateSequence(this);
            }
        }

        public long Sequence { get; set; }

        public string Prefix{
            get { return _prefix; }
        }
    }
}
