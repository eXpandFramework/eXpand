using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace XpandSystemTester.Module.BusinessObjects{
    [DefaultClassOptions]
    public class Customer : Person{
        private string _criteria;
        private string _objectTypeName;
        private readonly InitializeIndicator _initializeIndicator;

        public Customer(Session session)
            : base(session){
            _initializeIndicator=new InitializeIndicator();
        }
        [CriteriaOptions("DataType")]
        [EditorAlias(EditorAliases.CriteriaPropertyEditorEx)]
        [Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
        [VisibleInListView(true)]
        [ModelDefault("RowCount", "0")]
        public string Criteria {
            get { return _criteria; }
            set {
                SetPropertyValue("Criteria", ref _criteria, value);
            }
        }

        protected bool IsInitializing {
            get { return _initializeIndicator.IsInitializing; }
        }

        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        [ImmediatePostData, NonPersistent]
        public Type DataType {
            get {
                if (_objectTypeName != null) {
                    return ReflectionHelper.GetType(_objectTypeName);
                }
                return null;
            }
            set {
                string stringValue = value == null ? null : value.FullName;
                string savedObjectTypeName = ObjectTypeName;
                try {
                    if (stringValue != _objectTypeName) {
                        ObjectTypeName = stringValue;
                    }
                }
                catch (Exception) {
                    ObjectTypeName = savedObjectTypeName;
                }
                if (!IsInitializing) {
                    _criteria = null;
                }
            }
        }

        [Browsable(false)]
        public string ObjectTypeName {
            get {
                return _objectTypeName;
            }
            set {
                SetPropertyValue("ObjectTypeName", ref _objectTypeName, value);
            }
        }

        [Association, Aggregated]
        public XPCollection<SalesVolume> SalesVolumes{
            get { return GetCollection<SalesVolume>("SalesVolumes"); }
        }
    }

    [DefaultClassOptions]
    public class SalesVolume : BaseObject{
        private Customer _customer;
        private decimal _volume;

        private int _year;

        public SalesVolume(Session session)
            : base(session){
        }

        [Association]
        public Customer Customer{
            get { return _customer; }
            set { SetPropertyValue("Customer", ref _customer, value); }
        }

        public int Year{
            get { return _year; }
            set { SetPropertyValue("Year", ref _year, value); }
        }

        public decimal Volume{
            get { return _volume; }
            set { SetPropertyValue("Volume", ref _volume, value); }
        }

        [Association, Aggregated]
        public XPCollection<SalesVolumeMonth> Months{
            get { return GetCollection<SalesVolumeMonth>("Months"); }
        }
    }

    public class SalesVolumeMonth : BaseObject{
        private int _monthNumber;
        private decimal _volume;
        private SalesVolume _yearVolume;

        public SalesVolumeMonth(Session session)
            : base(session){
        }

        [Association]
        public SalesVolume YearVolume{
            get { return _yearVolume; }
            set { SetPropertyValue("YearVolume", ref _yearVolume, value); }
        }

        public int MonthNumber{
            get { return _monthNumber; }
            set { SetPropertyValue("MonthNumber", ref _monthNumber, value); }
        }

        public decimal Volume{
            get { return _volume; }
            set { SetPropertyValue("Volume", ref _volume, value); }
        }
    }
}