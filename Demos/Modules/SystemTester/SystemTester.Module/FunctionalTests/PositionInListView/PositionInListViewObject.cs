using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;

namespace SystemTester.Module.FunctionalTests.PositionInListView{
    [DefaultClassOptions]
    public class PositionInListViewObject : BaseObject,ISupportSequenceObject{
        private string _name;
        private int _position;

        public PositionInListViewObject(Session session) : base(session){
        }

        public string Name{
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        public int Position{
            get { return _position; }
            set { SetPropertyValue("Position", ref _position, value); }
        }

        protected override void OnSaving(){
            base.OnSaving();
            Xpand.Persistent.Base.General.SequenceGenerator.GenerateSequence(this);
        }

        long ISupportSequenceObject.Sequence{
            get { return Position; }
            set { Position = (int) value; }
        }

        string ISupportSequenceObject.Prefix{
            get { return null; }
        }
    }
}