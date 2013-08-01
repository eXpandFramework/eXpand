using System;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using XVideoRental.Module.Win.BusinessObjects.Movie;
using XVideoRental.Module.Win.DatabaseUpdate;
using Xpand.ExpressApp.IO.Core;
using Xpand.ExpressApp.Security.Core;
using Xpand.Persistent.Base.General.Model;

namespace XVideoRental.Module.Win.BusinessObjects.Rent {
    public enum ActiveRentType { None, Overdue, Today, Active }
    [InitialData(AllOwnMembers = true, BaseMembers = "oid|Oid,Id|RentId")]
    [CloneView(CloneViewType.ListView, "Rent_ListView_LateFees")]
    [CloneView(CloneViewType.ListView, "Rent_ListView_Purchaces")]
    [CloneView(CloneViewType.ListView, "Rent_ListView_Rentals")]
    [CloneView(CloneViewType.ListView, "Rent_ListView_Calendar")]
    [CloneView(CloneViewType.ListView, "Rent_ListView_KPI")]
    [CloneView(CloneViewType.ListView, "Rent_ListView_MediaPerformance")]
    [CloneView(CloneViewType.ListView, "Rent_ListView_MediaPerformance_Layout")]
    [CloneView(CloneViewType.ListView, ViewIdProvider.MediaPerformance_Movie_Summury)]
    [MapInheritance(MapInheritanceType.ParentTable)]
    [PermissionBehavior(PermissionBehavior.ReadOnlyAccess)]
    public class Rent : RentEvent {
        MovieItem item;
        int days;
        DateTime? returnedOn;
        Receipt receipt;
        Receipt receiptOverdue;
        decimal payment;
        decimal overduePayment;

        public Rent(Session session) : base(session) { }

        public Rent(Receipt receipt, MovieItem item, int days)
            : base(receipt.Session) {
            if (receipt == null) throw new ArgumentNullException("receipt");
            if (item == null) throw new ArgumentNullException("item");
            if (item.Status != MovieItemStatus.Active) throw new ArgumentException("Item is not active");
            Receipt = receipt;
            Item = item;
            if (receipt.Type == ReceiptType.Purchases) {
                Item.Status = MovieItemStatus.Sold;
                Item.AvailableForSell = false;
                Days = 0;
            } else {
                Item.Status = MovieItemStatus.Rented;
                Days = days != 0 ? days : item.Movie.Category.GetPrice(item.Format).DefaultRentDays;
            }
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            Days = 1;
        }

        public override string Subject {
            get { return Item.Movie.MovieTitle; }
            set { }
        }

        public override int Status {
            get { return (int)ActiveType; }
            set { }
        }

        public override string Description {
            get { return item.Movie.Plot; }
            set { }
        }

        public override DateTime StartOn {
            get { return RentedOn; }
            set { }
        }

        public override DateTime EndOn {
            get { return ExpectedOn; }
            set { }
        }

        public override int Label {
            get { return (int)item.Movie.Rating; }
            set { }
        }

        [PersistentAlias("Id")]
        public long RentId {
            get { return (long)EvaluateAlias("RentId"); }
        }
        [Persistent, Association("Item-Rents")]
        [RuleRequiredField]
        public MovieItem Item {
            get { return item; }
            protected set { SetPropertyValue("Item", ref item, value); }
        }

        public int Days {
            get { return days; }
            set { SetPropertyValue<int>("Days", ref days, value); }
        }

        public DateTime? ReturnedOn {
            get { return returnedOn; }
            set { SetPropertyValue("ReturnedOn", ref returnedOn, value); }
        }
        [Indexed(Unique = false)]
        public DateTime ExpectedOn { get { return ReturnedOn.HasValue ? ReturnedOn.Value : ClearedReturnedOn; } }
        public DateTime ClearedReturnedOn { get { return RentedOn.AddDays(Days); } }
        public DateTime ClearedReturnedOnDate { get { return ClearedReturnedOn.Date; } }
        public int Overdue {
            get {
                DateTime date = ReturnedOn.HasValue ? ReturnedOn.Value : DateTime.Now;
                return (int)(((double)date.Ticks - RentedOn.Ticks) / TimeSpan.TicksPerDay - Days);
            }
        }
        public ActiveRentType ActiveType {
            get {
                if (!Active) return ActiveRentType.None;
                if (Overdue > 0) return ActiveRentType.Overdue;
                if (Overdue == 0) return ActiveRentType.Today;
                return ActiveRentType.Active;
            }
        }
        public bool IsOverdue { get { return Overdue > 0; } }
        public bool Active { get { return ReturnedOn == null && Item.Status == MovieItemStatus.Rented; } }

        [Indexed(Unique = false), PersistentAlias("Receipt.Date")]
        public DateTime RentedOn {
            get { return (DateTime)EvaluateAlias("RentedOn"); }
        }
        protected override void OnSaving() {
            base.OnSaving();
            StartOn = RentedOn;
            EndOn = ReturnedOn == null ? DateTime.MinValue : ReturnedOn.Value;
        }
        public decimal Payment {
            get { return payment; }
            set { SetPropertyValue<decimal>("Payment", ref payment, value); }
        }
        public decimal OverduePayment {
            get { return overduePayment; }
            set { SetPropertyValue<decimal>("OverduePayment", ref overduePayment, value); }
        }

        public MovieItemFormat ItemFormat {
            get {
                return Item == null ? MovieItemFormat.DVD : Item.Format;
            }
        }

        [Persistent, Association("Receipt-Rents")]
        public Receipt Receipt {
            get { return receipt; }
            protected set { SetPropertyValue("Receipt", ref receipt, value); }
        }

        [Association("ReceiptOverdue-Rents")]
        public Receipt ReceiptOverdue {
            get { return receiptOverdue; }
            set { SetPropertyValue("ReceiptOverdue", ref receiptOverdue, value); }
        }

        [Action]
        public void Return() {
            if (Item.Status != MovieItemStatus.Rented) throw new Exception("Item is not in rent");
            ReturnedOn = DateTime.Now;
            Item.Status = MovieItemStatus.Active;
        }
        public decimal CalcPayment() {
            throw new NotImplementedException();
            //            if (Receipt.Type == ReceiptType.Purchases) Payment = Item.SellingPrice;
            //            else Payment = Item.CalcOnOrderPrice(Days);
            //            return Payment;
        }
        public decimal CalcOverduePayment() {
            throw new NotImplementedException();
            //            OverduePayment = Item.CalcOnOrderPrice(Days + Overdue) - Payment;
            //            return OverduePayment;
        }
    }
}
