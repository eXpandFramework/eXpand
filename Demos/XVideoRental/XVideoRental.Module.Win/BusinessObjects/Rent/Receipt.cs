using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using XVideoRental.Module.Win.BusinessObjects.Movie;
using XVideoRental.Module.Win.DatabaseUpdate;
using Xpand.ExpressApp.IO.Core;
using Xpand.ExpressApp.Security.Core;
using Xpand.Persistent.Base.General.Model;

namespace XVideoRental.Module.Win.BusinessObjects.Rent {



    public enum ReceiptType {
        Rentals,
        Purchases,
        LateFees
    }
    [InitialData(AllOwnMembers = true, BaseMembers = "oid|Oid,Id|ReceiptId")]
    [CloneView(CloneViewType.ListView, ViewIdProvider.CustomersKpi)]
    [CloneView(CloneViewType.ListView, ViewIdProvider.CustomersKpiDiscount)]
    [CloneView(CloneViewType.ListView, ViewIdProvider.CustomersKpiCustomersByDates)]
    [CloneView(CloneViewType.ListView, ViewIdProvider.StatisticsTopCustomers)]
    [CloneView(CloneViewType.ListView, ViewIdProvider.StatisticsRevenueSplit)]
    [CloneView(CloneViewType.ListView, ViewIdProvider.StatisticsRevenueByCustomer)]
    [CloneView(CloneViewType.ListView, ViewIdProvider.StatisticsNetIncomeReceipts)]
    [CloneView(CloneViewType.ListView, ViewIdProvider.StatisticsNetIncome)]
    [PermissionBehavior(PermissionBehavior.ReadOnlyAccess)]
    public class Receipt : VideoRentalBaseObject {
        private Customer _customer;
        private DateTime _date;
        private ReceiptType _type;
        decimal _payment;
        private decimal _discount;
        private bool _closed;

        public Receipt(Session session)
            : base(session) {
        }
        public Receipt(Customer customer, ReceiptType type)
            : base(customer.Session) {
            if (customer == null) throw new ArgumentNullException(nameof(customer));
            Customer = customer;
            Type = type;
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            Date = DateTime.Now;
        }

        [PersistentAlias("Id")]
        public long ReceiptId => (long)EvaluateAlias("ReceiptId");

        [Persistent, Association("Customer-Receipts")]
        [RuleRequiredField]
        public Customer Customer {
            get { return _customer; }
            protected set { SetPropertyValue("Customer", ref _customer, value); }
        }

        [Indexed(Unique = false)]
        public DateTime Date {
            get { return _date; }
            set { SetPropertyValue<DateTime>("Date", ref _date, value); }
        }

        [Persistent]
        public ReceiptType Type {
            get { return _type; }
            protected set { SetPropertyValue("Type", ref _type, value); }
        }

        public decimal Payment {
            get { return _payment; }
            set { SetPropertyValue<decimal>("Payment", ref _payment, value); }
        }

        public decimal Discount {
            get { return _discount; }
            set { SetPropertyValue<decimal>("Discount", ref _discount, value); }
        }

        [Persistent]
        public bool Closed {
            get { return _closed; }
            protected set { SetPropertyValue("Closed", ref _closed, value); }
        }

        [Association("Receipt-Rents")]
        public XPCollection<Rent> Rents => GetCollection<Rent>("Rents");

        //Type = Rent or Sale
        [Association("ReceiptOverdue-Rents")]
        public XPCollection<Rent> OverdueRents => GetCollection<Rent>("OverdueRents");

        //Type = Overdue
        public void CalcPayment() {
            if (Type == ReceiptType.LateFees) CalcOverduePayment();
            else CalcOrdinarPayment();
            Customer.CalcDiscount();
        }

        public Receipt Buy(ICollection<RentInfo> rentsInfo) {
            Receipt receipt = null;
            foreach (RentInfo rentInfo in rentsInfo) {
                MovieItem item = rentInfo.Item.SellItem;
                if (item == null) continue;
                if (receipt == null) receipt = new Receipt(Customer, ReceiptType.Purchases);
                new Rent(receipt, item, rentInfo.Days);
            }
            receipt?.CalcPayment();
            return receipt;
        }

        void CalcOrdinarPayment() {
            decimal payment = Rents.Sum(rent => rent.CalcPayment());
            CalcDiscount(payment);
        }

        void CalcOverduePayment() {
            decimal payment = OverdueRents.Sum(rent => rent.CalcOverduePayment());
            CalcDiscount(payment);
        }

        void CalcDiscount(decimal payment) {
            Discount = Math.Round(payment * Customer.Discount * 100) / 100;
            Payment = payment - Discount;
        }
    }
    public interface IRentItem {
        MovieItem RentItem { get; }
        MovieItem SellItem { get; }
    }

    public class RentInfo {
        public RentInfo(IRentItem item, int days) {
            Item = item;
            Days = days;
        }
        public RentInfo(IRentItem item) : this(item, 0) { }
        public RentInfo(Movie.Movie movie, MovieItemFormat format, int days) : this(new MovieInFormat(movie, format), days) { }
        public RentInfo(Movie.Movie movie, MovieItemFormat format) : this(new MovieInFormat(movie, format)) { }
        public IRentItem Item { get; }
        public int Days { get; }
    }
    public class MovieInFormat : IRentItem {
        public MovieInFormat(Movie.Movie movie, MovieItemFormat format) {
            Movie = movie;
            Format = format;
        }
        public Movie.Movie Movie { get; }
        public MovieItemFormat Format { get; }

        public MovieItem RentItem {
            get {
                var items = new XPCollection<MovieItem>(Movie.Items, CriteriaOperator.Parse("Status = ? and Format = ?", MovieItemStatus.Active, Format));
                return items.Count == 0 ? null : items[0];
            }
        }
        public MovieItem SellItem {
            get {
                var items = new XPCollection<MovieItem>(Movie.Items,
                    CriteriaOperator.Parse("Status = ? and AvailableForSell = ? and Format = ?", MovieItemStatus.Active, true, Format));
                return items.Count == 0 ? null : items[0];
            }
        }
    }

}