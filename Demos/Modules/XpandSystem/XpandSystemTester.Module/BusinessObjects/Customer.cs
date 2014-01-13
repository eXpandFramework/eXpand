using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace XpandSystemTester.Module.BusinessObjects{
    [DefaultClassOptions]
    public class Customer : Person{
        public Customer(Session session)
            : base(session){
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