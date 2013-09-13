using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.MapView;

namespace MapViewTester.Module.BusinessObjects
{
    [DefaultClassOptions]
    public class Customer : Person, IMapAddress, IMapInfoWindow
    {
        public Customer(Session session)
            : base(session)
        {
        }

        [Association, Aggregated]
        public XPCollection<SalesVolume> SalesVolumes { get { return GetCollection<SalesVolume>("SalesVolumes"); } }


        string IMapAddress.Address
        {
            get
            {
                if (Address1 != null)
                    return string.Format(CultureInfo.InvariantCulture, "{0}, {1} {2}", Address1.Street,
                                         Address1.ZipPostal, Address1.City);

                return string.Empty;
            }
        }



        [NonPersistent]
        [VisibleInDetailView(false)]
        public string InfoWindowText
        {
            get { return FullName; }
        }
    }

    public class SalesVolume : BaseObject
    {
        public SalesVolume(Session session)
            : base(session)
        {

        }

        private Customer customer;
        [Association]
        public Customer Customer
        {
            get { return customer; }
            set { SetPropertyValue("Customer", ref customer, value); }
        }

        private int year;
        public int Year
        {
            get { return year; }
            set { SetPropertyValue("Year", ref year, value); }
        }

        private decimal volume;
        public decimal Volume
        {
            get { return volume; }
            set { SetPropertyValue("Volume", ref volume, value); }
        }

        [Association, Aggregated]
        public XPCollection<SalesVolumeMonth> Months { get { return GetCollection<SalesVolumeMonth>("Months"); } }

    }

    public class SalesVolumeMonth : BaseObject
    {
        public SalesVolumeMonth(Session session)
            : base(session)
        {

        }

        private SalesVolume yearVolume;
        [Association]
        public SalesVolume YearVolume
        {
            get { return yearVolume; }
            set { SetPropertyValue("YearVolume", ref yearVolume, value); }
        }

        private int monthNumber;
        public int MonthNumber
        {
            get { return monthNumber; }
            set { SetPropertyValue("MonthNumber", ref monthNumber, value); }
        }

        private decimal volume;
        public decimal Volume
        {
            get { return volume; }
            set { SetPropertyValue("Volume", ref volume, value); }
        }
    }

}
