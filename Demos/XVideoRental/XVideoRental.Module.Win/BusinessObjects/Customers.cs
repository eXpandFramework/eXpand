using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.Validation;
using DevExpress.Utils;
using DevExpress.Xpo;
using XVideoRental.Module.Win.BusinessObjects.Movie;
using XVideoRental.Module.Win.BusinessObjects.Rent;
using Xpand.ExpressApp.Attributes;
using Xpand.ExpressApp.IO.Core;
using Xpand.ExpressApp.Security.Core;
using Xpand.ExpressApp.Win.SystemModule.ToolTip;
using Xpand.Utils.Helpers;

namespace XVideoRental.Module.Win.BusinessObjects {
    public enum DiscountLevel {
        FirstTime,
        Basic,
        Occasional,
        Active,
        Prodigious
    }

    public enum Gender {
        [ImageName("male")]
        Male,
        [ImageName("female")]
        Female
    }
    [DefaultClassOptions]
    [InitialData(Name = "vCustomer", BaseMembers = "oid|Oid,Id|CustomerId,FirstName,LastName,Birthday|BirthDate,Email,Photo,Address,MiddleName")]
    [FullPermission]
    [CloneView(CloneViewType.ListView, "Customer_ListView_AdvBanded")]
    [CloneView(CloneViewType.DetailView, "Customer_DetailView_Rentals")]
    [CloneView(CloneViewType.ListView, "Customer_ListView_Rentals", DetailView = "Customer_DetailView_Rentals")]
    [CloneView(CloneViewType.ListView, "Customer_ListView_Calendar")]
    [CloneView(CloneViewType.ListView, "Customer_ListView_Reports_MovieRentals")]
    [CloneView(CloneViewType.ListView, "Customer_ListView_Reports_CustomerCards")]
    [DefaultProperty("FullName")]
    [FriendlyKeyProperty("FullName")]
    public class Customer : VideoPerson, IResource {
        public static decimal[] CustomerDiscount = new[] { 0, 0.03M, 0.05M, 0.1M, 0.2M };
        string comments;
        private string _phone;

        public Customer(Session session)
            : base(session) {

        }
        public Receipt DoRent(ICollection<RentInfo> rentsInfo) {
            Receipt receipt = null;
            foreach (RentInfo rentInfo in rentsInfo) {
                MovieItem item = rentInfo.Item.RentItem;
                if (item == null) continue;
                if (receipt == null) receipt = new Receipt(this, ReceiptType.Rentals);
                new Rent.Rent(receipt, item, rentInfo.Days);
            }
            if (receipt != null) receipt.CalcPayment();
            return receipt;
        }

        public decimal Discount { get { return (decimal)CustomerDiscount.GetValue((int)DiscountLevel); } }
        [PersistentAlias("Id")]
        public long CardNumber {
            get { return (long)EvaluateAlias("CardNumber"); }
        }
        [PersistentAlias("Id")]
        public long BarCode {
            get { return (long)EvaluateAlias("BarCode"); }
        }

        private DiscountLevel _discountLevel;
        [InitialData]
        public DiscountLevel DiscountLevel {
            get { return _discountLevel; }
            set { SetPropertyValue("DiscountLevel", ref _discountLevel, value); }
        }

        [InitialData]
        [RuleRequiredField]
        public string Phone {
            get { return _phone; }
            set { SetPropertyValue("Phone", ref _phone, value); }
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            color = Color.Red.ToArgb();
        }

        [Size(SizeAttribute.Unlimited)]
        [InitialData]
        public string Comments {
            get { return comments; }
            set { SetPropertyValue("Comments", ref comments, value); }
        }

        [Association("Customer-Receipts")]
        public XPCollection<Receipt> Receipts {
            get { return GetCollection<Receipt>("Receipts"); }
        }

        public XPCollection<Rent.Rent> ActiveRents {
            get {
                CriteriaOperator criteriaOperator = CriteriaOperator.Parse("Receipt.Customer = ? and Active = ?", this, true);
                return new XPCollection<Rent.Rent>(Session, criteriaOperator) { BindingBehavior = CollectionBindingBehavior.AllowNone };
            }
        }

        #region Implementation of IResource
        [NonPersistent, Browsable(false)]
        object IResource.Id {
            get { return Id; }
        }


        int color;

        [NonPersistent]
        public string Caption {
            get { return FullName; }
            set { throw new NotImplementedException(); }
        }

        [NonPersistent, Browsable(false)]
        public Int32 OleColor {
            get { return ColorTranslator.ToOle(Color.FromArgb(color)); }
        }

        [NonPersistent]
        public Color Color {
            get { return Color.FromArgb(color); }
            set {
                color = value.ToArgb();
                OnChanged("Color");
            }
        }
        #endregion
        public void CalcDiscount() {
            decimal paymentSum = GetPaymentSum();
            DiscountLevel = new DiscountLevelCalculator().Calculate(paymentSum);
        }
        public decimal GetPaymentSum() {
            return Receipts.Sum(reciept => reciept.Payment);
        }

        public string GetCustomerInfoHtml() {
            string ret = string.Format("<b>{0}</b>", FullName);
            ret += string.Format("\r\n{1}: <b>{0:d6}</b>", Id, CaptionHelper.GetMemberCaption(GetType(), "CardNumber"));
            if (Birthday != DateTime.MinValue) ret += string.Format("\r\n{1}: <b>{0:d}</b>", Birthday, CaptionHelper.GetMemberCaption(GetType(), "BirthDate"));

            if (!string.IsNullOrEmpty(Email)) ret += string.Format("\r\n{1}: <b>{0}</b>", Email, CaptionHelper.GetMemberCaption(GetType(), "Email"));
            if (!string.IsNullOrEmpty(Phone)) ret += string.Format("\r\n{1}: <b>{0}</b>", Phone, CaptionHelper.GetMemberCaption(GetType(), "Phone"));
            if (!string.IsNullOrEmpty(Address)) ret += string.Format("\r\n{1}: <b>{0}</b>", Address, CaptionHelper.GetMemberCaption(GetType(), "Address"));
            ret += string.Format("\r\n{2}: <b>{0}({1:p})</b>", DiscountLevel.ToString(), Discount, CaptionHelper.GetMemberCaption(GetType(), "DiscountLevel"));
            if (!string.IsNullOrEmpty(Comments)) ret += string.Format("\r\n{1}: <i>{0}</i>", Comments, CaptionHelper.GetMemberCaption(GetType(), "Comments"));
            return ret;
        }
    }
    public class DiscountLevelCalculator {
        public static decimal[] CustomerDiscountLevel = new decimal[] { 0, 300, 600, 900, 1200 };

        public DiscountLevel Calculate(decimal summary) {
            if (summary > CustomerDiscountLevel[(int)DiscountLevel.Prodigious])
                return DiscountLevel.Prodigious;
            if (summary > CustomerDiscountLevel[(int)DiscountLevel.Active])
                return DiscountLevel.Active;
            if (summary > CustomerDiscountLevel[(int)DiscountLevel.Occasional])
                return DiscountLevel.Occasional;
            if (summary > CustomerDiscountLevel[(int)DiscountLevel.Basic])
                return DiscountLevel.Basic;
            return DiscountLevel.FirstTime;
        }
    }

    public class CustomerToolTipController : ObjectToolTipController {
        const int MaxPhotoWidth = 120, MaxPhotoHeight = 120;
        public CustomerToolTipController(Control parent) : base(parent) { }

        protected override void InitToolTipItem(ToolTipItem item) {
            var customer = ((Customer)EditObject);
            var photo = customer.Photo;
            if (photo != null)
                item.Image = photo.CreateImage(MaxPhotoWidth, MaxPhotoHeight);
            item.Text = customer.GetCustomerInfoHtml();
        }
    }

}