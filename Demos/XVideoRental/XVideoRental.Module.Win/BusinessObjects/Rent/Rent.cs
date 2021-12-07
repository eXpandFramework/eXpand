using System;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using XVideoRental.Module.Win.BusinessObjects.Movie;
using XVideoRental.Module.Win.DatabaseUpdate;
using Xpand.ExpressApp.IO.Core;
using Xpand.ExpressApp.Security.Core;
using Xpand.Persistent.Base.General.Model;
using Xpand.XAF.Modules.CloneModelView;

namespace XVideoRental.Module.Win.BusinessObjects.Rent {
    public enum ActiveRentType { None, Overdue, Today, Active }
    [InitialData(AllOwnMembers = true, BaseMembers = "oid|Oid,Id|RentId")]
    [CloneModelView(CloneViewType.ListView, "Rent_ListView_LateFees")]
    [CloneModelView(CloneViewType.ListView, "Rent_ListView_Purchaces")]
    [CloneModelView(CloneViewType.ListView, "Rent_ListView_Rentals")]
    [CloneModelView(CloneViewType.ListView, "Rent_ListView_Calendar")]
    [CloneModelView(CloneViewType.ListView, "Rent_ListView_KPI")]
    [CloneModelView(CloneViewType.ListView, "Rent_ListView_MediaPerformance")]
    [CloneModelView(CloneViewType.ListView, "Rent_ListView_MediaPerformance_Layout")]
    [CloneModelView(CloneViewType.ListView, ViewIdProvider.MediaPerformanceMovieSummury)]
    [MapInheritance(MapInheritanceType.ParentTable)]
    [PermissionBehavior(PermissionBehavior.ReadOnlyAccess)]
    public class Rent : RentEvent {
        MovieItem _item;
        int _days;
        DateTime? _returnedOn;
        Receipt _receipt;
        Receipt _receiptOverdue;
        decimal _payment;
        decimal _overduePayment;

        public Rent(Session session) : base(session) { }

        public Rent(Receipt receipt, MovieItem item, int days)
            : base(receipt.Session) {
            if (receipt == null) throw new ArgumentNullException(nameof(receipt));
            if (item == null) throw new ArgumentNullException(nameof(item));
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "XAF0002:XPO business class properties should not be overriden", Justification = "<Pending>")]
        public override string Subject {
            get {
                return Item?.Movie.MovieTitle;
            }
            set { }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "XAF0002:XPO business class properties should not be overriden", Justification = "<Pending>")]
        public override int Status {
            get { return (int)ActiveType; }
            set { }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "XAF0002:XPO business class properties should not be overriden", Justification = "<Pending>")]
        public override string Description {
            get {
                return _item?.Movie.Plot;
            }
            set { }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "XAF0002:XPO business class properties should not be overriden", Justification = "<Pending>")]
        public override DateTime StartOn {
            get { return RentedOn; }
            set { }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "XAF0002:XPO business class properties should not be overriden", Justification = "<Pending>")]
        public override DateTime EndOn {
            get { return ExpectedOn; }
            set { }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "XAF0002:XPO business class properties should not be overriden", Justification = "<Pending>")]
        public override int Label {
            get { return (int)_item.Movie.Rating; }
            set { }
        }

        [PersistentAlias("Id")]
        public long RentId => (long)EvaluateAlias("RentId");

        [Persistent, Association("Item-Rents")]
        [RuleRequiredField]
        public MovieItem Item {
            get { return _item; }
            protected set { SetPropertyValue("Item", ref _item, value); }
        }

        public int Days {
            get { return _days; }
            set { SetPropertyValue<int>("Days", ref _days, value); }
        }

        public DateTime? ReturnedOn {
            get { return _returnedOn; }
            set { SetPropertyValue("ReturnedOn", ref _returnedOn, value); }
        }
        [Indexed(Unique = false)]
        public DateTime ExpectedOn => ReturnedOn ?? ClearedReturnedOn;

        public DateTime ClearedReturnedOn => RentedOn.AddDays(Days);
        public DateTime ClearedReturnedOnDate => ClearedReturnedOn.Date;

        public int Overdue {
            get {
                DateTime date = ReturnedOn ?? DateTime.Now;
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
        public bool IsOverdue => Overdue > 0;
        public bool Active => ReturnedOn == null && Item.Status == MovieItemStatus.Rented;

        [Indexed(Unique = false), PersistentAlias("Receipt.Date")]
        public DateTime RentedOn => (DateTime)EvaluateAlias("RentedOn");

        protected override void OnSaving() {
            base.OnSaving();
            StartOn = RentedOn;
            EndOn = ReturnedOn ?? DateTime.MinValue;
        }
        public decimal Payment {
            get { return _payment; }
            set { SetPropertyValue<decimal>("Payment", ref _payment, value); }
        }
        public decimal OverduePayment {
            get { return _overduePayment; }
            set { SetPropertyValue<decimal>("OverduePayment", ref _overduePayment, value); }
        }

        public MovieItemFormat ItemFormat => Item?.Format ?? MovieItemFormat.DVD;

        [Persistent, Association("Receipt-Rents")]
        public Receipt Receipt {
            get { return _receipt; }
            protected set { SetPropertyValue("Receipt", ref _receipt, value); }
        }

        [Association("ReceiptOverdue-Rents")]
        public Receipt ReceiptOverdue {
            get { return _receiptOverdue; }
            set { SetPropertyValue("ReceiptOverdue", ref _receiptOverdue, value); }
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
