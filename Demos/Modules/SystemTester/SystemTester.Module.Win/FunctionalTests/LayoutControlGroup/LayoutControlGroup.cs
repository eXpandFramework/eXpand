using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SystemTester.Module.Win.FunctionalTests.LayoutControlGroup{
    [DefaultClassOptions]
    public class LayoutControlGroupObject : BaseObject{
        private string _sizeableEditor1;
        private string _sizeableEditor2;
        private string _simpleEditor1;
        private string _simpleEditor2;

        public LayoutControlGroupObject(Session session)
            : base(session){
        }


        public string SimpleEditor2{
            get { return _simpleEditor2; }
            set { SetPropertyValue("SimpleEditor2", ref _simpleEditor2, value); }
        }

        public string SimpleEditor1{
            get { return _simpleEditor1; }
            set { SetPropertyValue("SimpleEditor1", ref _simpleEditor1, value); }
        }

        [Size(SizeAttribute.Unlimited)]
        public string SizeableEditor1{
            get { return _sizeableEditor1; }
            set { SetPropertyValue("SizeableEditor1", ref _sizeableEditor1, value); }
        }
        [Size(SizeAttribute.Unlimited)]
        public string SizeableEditor2{
            get { return _sizeableEditor2; }
            set { SetPropertyValue("SizeableEditor2", ref _sizeableEditor2, value); }
        }
    }
}