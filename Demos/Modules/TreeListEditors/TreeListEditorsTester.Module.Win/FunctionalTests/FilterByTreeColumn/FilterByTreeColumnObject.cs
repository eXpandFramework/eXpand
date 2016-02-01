using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace TreeListEditorsTester.Module.Win.FunctionalTests.FilterByTreeColumn {
    [DefaultClassOptions]
    public class FilterByTreeColumnObject : HCategory {
        public FilterByTreeColumnObject(Session session) : base(session) {
        }
    }


    public class FilterByTreeColumnObjectDerived : FilterByTreeColumnObject {
        private string _derivedProperty;

        public FilterByTreeColumnObjectDerived(Session session) : base(session) {
        }

        public string DerivedProperty {
            get { return _derivedProperty; }
            set { SetPropertyValue("DerivedProperty", ref _derivedProperty, value); }
        }
    }

}