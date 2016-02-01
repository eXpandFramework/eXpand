using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SystemTester.Module.Win.FunctionalTests.FilterByColumn{
    [DefaultClassOptions]
    public class FilterByColumnObject : BaseObject{
        private string _name;

        public FilterByColumnObject(Session session) : base(session){
        }

        public string Name{
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }
    }

    
    public class FilterByColumnObjectDerived : FilterByColumnObject{
        private string _derivedProperty;

        public FilterByColumnObjectDerived(Session session) : base(session){
        }

        public string DerivedProperty{
            get { return _derivedProperty; }
            set { SetPropertyValue("DerivedProperty", ref _derivedProperty, value); }
        }
    }

}