using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.IO.Core;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.BaseImpl.Security;

namespace XVideoRental.Module.Win.BusinessObjects {
    public class ViewIdProvider {
        public const string MovieItemMediaPerformance = "MovieItem_ListView_MediaPerformance";
        public const string MediaPerformanceMovieSummury = "Rent_ListView_MediaPerformance_Movie_Summury";
        public const string CustomersKpi = "Receipt_ListView_KPI";
        public const string CustomersKpiDiscount = "Receipt_ListView_KPI_Discount";
        public const string CustomersKpiCustomersByDates = "Receipt_ListView_KPI_Customers_By_Dates";
        public const string StatisticsTopCustomers = "Receipt_ListView_Statistics_TopCustomers";
        public const string StatisticsRevenueSplit = "Receipt_ListView_Statistics_Revenue_Split";
        public const string StatisticsRevenueByCustomer = "Receipt_ListView_Statistics_Revenue_ByCustomer";
        public const string StatisticsNetIncome = "Receipt_ListView_Statistics_Net_Income";
        public const string StatisticsNetIncomeReceipts = "Receipt_ListView_Statistics_Net_Income_Receipts";
    }
    [NonPersistent]
    public abstract class VideoRentalBaseObject : SequenceBaseObject {
        DateTime _createdAt;
        XpandPermissionPolicyUser _createdBy;
        string _tag;

        protected VideoRentalBaseObject(Session session)
            : base(session) {
        }
        [Browsable(false)]
        public XpandPermissionPolicyUser CreatedBy {
            get { return _createdBy; }
            set { SetPropertyValue("CreatedBy", ref _createdBy, value); }
        }
        [Browsable(false)]
        public DateTime CreatedAt {
            get { return _createdAt; }
            set { SetPropertyValue<DateTime>("CreatedAt", ref _createdAt, value); }
        }

        [Size(SizeAttribute.Unlimited)]
        [Browsable(false)]
        public string Tag {
            get { return _tag; }
            set { SetPropertyValue("Tag", ref _tag, value); }
        }


        public override void AfterConstruction() {
            base.AfterConstruction();
            IObjectSpace objectSpace = XPObjectSpace.FindObjectSpaceByObject(this);
            if (objectSpace != null) CreatedBy = (XpandPermissionPolicyUser)objectSpace.GetObject(SecuritySystem.CurrentUser);
        }
    }

    public class SequenceBaseObject : BaseObject {
        internal static bool Updating;
        long _id;

        public SequenceBaseObject(Session session)
            : base(session) {
        }

        [Browsable(false)]
        [Indexed(Unique = false)]
        public long Id {
            get { return _id; }
            set { SetPropertyValue("Id", ref _id, value); }
        }


    }

    [NonPersistent]
    public abstract class VideoPerson : VideoRentalBaseObject {
        string _address;
        Gender _gender;
        string _middleName;
        protected VideoPerson(Session session)
            : base(session) {
        }
        #region Person
        const string DefaultFullNameFormat = "{LastName} {MiddleName} {FirstName}";
        const string DefaultFullNamePersistentAlias = "concat(FirstName,' ',MiddleName,' ', LastName)";
        readonly PersonImpl _person = new PersonImpl();


        static VideoPerson() {
            PersonImpl.FullNameFormat = DefaultFullNameFormat;
        }

        public string MiddleName {
            get { return _middleName; }
            set { SetPropertyValue("MiddleName", ref _middleName, value); }
        }

        [InitialData]
        public Gender Gender {
            get { return _gender; }
            set { SetPropertyValue("Gender", ref _gender, value); }
        }



        public void SetFullName(string fullName) {
            _person.SetFullName(fullName);
        }
        [RuleRequiredField]
        public string FirstName {
            get { return _person.FirstName; }
            set {
                _person.FirstName = value;
                OnChanged("FirstName");
            }
        }
        [RuleRequiredField]
        public string LastName {
            get { return _person.LastName; }
            set {
                _person.LastName = value;
                OnChanged("LastName");
            }
        }

        [DevExpress.Xpo.DisplayName("BirthDate")]
        public DateTime Birthday {
            get { return _person.Birthday; }
            set {
                _person.Birthday = value;
                OnChanged("Birthday");
            }
        }

        //        [ObjectValidatorIgnoreIssue(typeof(ObjectValidatorDefaultPropertyIsNonPersistentNorAliased)),
        //         SearchMemberOptions(SearchMemberMode.Include)]
        [PersistentAlias(DefaultFullNamePersistentAlias)]
        public string FullName => EvaluateAlias("FullName") as string;

        [Size(255)]
        public string Email {
            get { return _person.Email; }
            set {
                _person.Email = value;
                OnChanged("Email");
            }
        }

        [Size(SizeAttribute.Unlimited), Delayed(true), ValueConverter(typeof(ImageValueConverter))]
        public Image Photo {
            get { return GetDelayedPropertyValue<Image>("Photo"); }
            set { SetDelayedPropertyValue("Photo", value); }
        }



        [Size(SizeAttribute.Unlimited)]
        [RuleRequiredField]
        public string Address {
            get { return _address; }
            set { SetPropertyValue("Address", ref _address, value); }
        }
        #endregion
    }

}
