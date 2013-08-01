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
    [CloneView(CloneViewType.ListView, ViewIdProvider.CustomersKPI)]
    [CloneView(CloneViewType.ListView, ViewIdProvider.CustomersKPIDiscount)]
    [CloneView(CloneViewType.ListView, ViewIdProvider.CustomersKPICustomersByDates)]
    [CloneView(CloneViewType.ListView, ViewIdProvider.StatisticsTopCustomers)]
    [CloneView(CloneViewType.ListView, ViewIdProvider.StatisticsRevenueSplit)]
    [CloneView(CloneViewType.ListView, ViewIdProvider.StatisticsRevenueByCustomer)]
    [CloneView(CloneViewType.ListView, ViewIdProvider.StatisticsNetIncomeReceipts)]
    [CloneView(CloneViewType.ListView, ViewIdProvider.StatisticsNetIncome)]
    [PermissionBehavior(PermissionBehavior.ReadOnlyAccess)]
    public class Receipt : VideoRentalBaseObject {
        Customer customer;
        DateTime date;
        ReceiptType type;
        decimal _payment;
        decimal discount;
        bool closed;

        public Receipt(Session session)
            : base(session) {
        }
        public Receipt(Customer customer, ReceiptType type)
            : base(customer.Session) {
            if (customer == null) throw new ArgumentNullException("customer");
            Customer = customer;
            Type = type;
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            Date = DateTime.Now;
        }

        [PersistentAlias("Id")]
        public long ReceiptId {
            get { return (long)EvaluateAlias("ReceiptId"); }
        }

        [Persistent, Association("Customer-Receipts")]
        [RuleRequiredField]
        public Customer Customer {
            get { return customer; }
            protected set { SetPropertyValue("Customer", ref customer, value); }
        }

        [Indexed(Unique = false)]
        public DateTime Date {
            get { return date; }
            set { SetPropertyValue<DateTime>("Date", ref date, value); }
        }

        [Persistent]
        public ReceiptType Type {
            get { return type; }
            protected set { SetPropertyValue("Type", ref type, value); }
        }

        public decimal Payment {
            get { return _payment; }
            set { SetPropertyValue<decimal>("Payment", ref _payment, value); }
        }

        public decimal Discount {
            get { return discount; }
            set { SetPropertyValue<decimal>("Discount", ref discount, value); }
        }

        [Persistent]
        public bool Closed {
            get { return closed; }
            protected set { SetPropertyValue("Closed", ref closed, value); }
        }

        [Association("Receipt-Rents")]
        public XPCollection<Rent> Rents {
            get { return GetCollection<Rent>("Rents"); }
        }

        //Type = Rent or Sale
        [Association("ReceiptOverdue-Rents")]
        public XPCollection<Rent> OverdueRents {
            get { return GetCollection<Rent>("OverdueRents"); }
        }

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
            if (receipt != null) receipt.CalcPayment();
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
        readonly IRentItem item;
        readonly int days;
        public RentInfo(IRentItem item, int days) {
            this.item = item;
            this.days = days;
        }
        public RentInfo(IRentItem item) : this(item, 0) { }
        public RentInfo(Movie.Movie movie, MovieItemFormat format, int days) : this(new MovieInFormat(movie, format), days) { }
        public RentInfo(Movie.Movie movie, MovieItemFormat format) : this(new MovieInFormat(movie, format)) { }
        public IRentItem Item { get { return item; } }
        public int Days { get { return days; } }
    }
    public class MovieInFormat : IRentItem {
        readonly Movie.Movie movie;
        readonly MovieItemFormat format;
        public MovieInFormat(Movie.Movie movie, MovieItemFormat format) {
            this.movie = movie;
            this.format = format;
        }
        public Movie.Movie Movie { get { return movie; } }
        public MovieItemFormat Format { get { return format; } }
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