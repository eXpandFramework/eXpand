using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General.CustomAttributes;

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

        ColumnChooserReference2 _columnChooserReference2;
        [VisibleInListView(false)]
        public ColumnChooserReference2 ColumnChooserReference2{
            get { return _columnChooserReference2; }
            set { SetPropertyValue(nameof(ColumnChooserReference2), ref _columnChooserReference2, value); }
        }
        
        public ColumnChooserReference ColumnChooserReference{
            get { return _columnChooserReference; }
            set { SetPropertyValue(nameof(ColumnChooserReference), ref _columnChooserReference, value); }
        }
    }

    public class ColumnChooserReference2:BaseObject{
        public ColumnChooserReference2(Session session) : base(session){
        }

        string _referenceName;

        public string ReferenceName{
            get { return _referenceName; }
            set { SetPropertyValue(nameof(ReferenceName), ref _referenceName, value); }
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