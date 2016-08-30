using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SystemTester.Module.Web.FunctionalTests.ColumnChooser{
    [DefaultClassOptions]
    public class ColumnChooserObject : BaseObject{
        private ColumnChooserReference _columnChooserReference;

        private string _name;

        public ColumnChooserObject(Session session) : base(session){
        }

        public string Name{
            get { return _name; }
            set { SetPropertyValue(nameof(Name), ref _name, value); }
        }

        public ColumnChooserReference ColumnChooserReference{
            get { return _columnChooserReference; }
            set { SetPropertyValue(nameof(ColumnChooserReference), ref _columnChooserReference, value); }
        }
    }


    public class ColumnChooserReference : BaseObject{
        private string _referenceName;

        public ColumnChooserReference(Session session) : base(session){
        }

        public string ReferenceName{
            get { return _referenceName; }
            set { SetPropertyValue(nameof(ReferenceName), ref _referenceName, value); }
        }

        ColumnChooserReferenceReference _columnChooserReferenceReference;

        public ColumnChooserReferenceReference ColumnChooserReferenceReference{
            get { return _columnChooserReferenceReference; }
            set { SetPropertyValue(nameof(ColumnChooserReferenceReference), ref _columnChooserReferenceReference, value); }
        }
    }

    public class ColumnChooserReferenceReference : BaseObject{
        private string _referenceName;

        public ColumnChooserReferenceReference(Session session) : base(session){
        }

        public string ReferenceReferenceName{
            get { return _referenceName; }
            set { SetPropertyValue(nameof(ReferenceReferenceName), ref _referenceName, value); }
        }
    }
}