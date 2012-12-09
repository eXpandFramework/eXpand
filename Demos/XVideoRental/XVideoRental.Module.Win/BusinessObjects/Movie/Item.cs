using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.Attributes;
using Xpand.ExpressApp.IO.Core;
using Xpand.ExpressApp.Security.Core;

namespace XVideoRental.Module.Win.BusinessObjects.Movie {
    [FullPermission]
    [InitialData(BaseMembers = "oid|Oid,Id|MovieItemId", AllOwnMembers = true)]
    [CloneView(CloneViewType.ListView, ViewIdProvider.MovieItemMediaPerformance)]
    [VisibleInReports]
    public class MovieItem : VideoRentalBaseObject {
        Movie movie;
        MovieItemStatus status;
        MovieItemFormat format;
        decimal sellingPrice;
        bool availableForSellMark;
        string location;

        public MovieItem(Session session) : base(session) { }

        public override void AfterConstruction() {
            base.AfterConstruction();
            Status = MovieItemStatus.Active;
            Format = MovieItemFormat.DVD;
            AvailableForSell = false;
        }

        [PersistentAlias("Id")]
        public long MovieItemId {
            get { return (long)EvaluateAlias("MovieItemId"); }

        }

        public MovieItem RentItem {
            get {
                return Status == MovieItemStatus.Active ? this : null;
            }
        }
        [Association("Item-Rents")]
        public XPCollection<Rent.Rent> Rents {
            get { return GetCollection<Rent.Rent>("Rents"); }
        }
        public MovieItem SellItem {
            get { return AvailableForSell && Status == MovieItemStatus.Active ? this : null; }
        }
        [Association("Movie-Items")]
        public Movie Movie {
            get { return movie; }
            set { SetPropertyValue("Movie", ref movie, value); }
        }

        public MovieItemStatus Status {
            get { return status; }
            set { SetPropertyValue("Status", ref status, value); }
        }

        public MovieItemFormat Format {
            get { return format; }
            set { SetPropertyValue("Format", ref format, value); }
        }

        public decimal SellingPrice {
            get { return sellingPrice; }
            set { SetPropertyValue<decimal>("SellingPrice", ref sellingPrice, value); }
        }

        [Size(SizeAttribute.Unlimited)]
        public string Location {
            get { return location; }
            set { SetPropertyValue("Location", ref location, value); }
        }
        [Persistent]
        protected bool AvailableForSellMark {
            get { return availableForSellMark; }
            set { SetPropertyValue("AvailableForSellMark", ref availableForSellMark, value); }
        }

        [NonPersistent]
        public bool AvailableForSell {
            get { return AvailableForSellMark && Status != MovieItemStatus.Sold; }
            set { AvailableForSellMark = value; }
        }

    }
}