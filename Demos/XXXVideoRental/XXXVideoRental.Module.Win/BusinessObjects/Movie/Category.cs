using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System.Linq;
using Xpand.ExpressApp.IO.Core;
using Xpand.ExpressApp.Security.Core;

namespace XXXVideoRental.Module.Win.BusinessObjects.Movie {
    [FullPermission]
    [InitialData(BaseMembers = "oid|Oid", AllOwnMembers = true)]
    [ImageName("BO_Categories")]
    public class MovieCategory : VideoRentalBaseObject {
        string name;
        bool isDefault;

        public MovieCategory(Session session) : base(session) { }
        [Indexed(Unique = true)]
        [RuleRequiredField]
        public string Name {
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
        }

        [Association("Category-Movies")]
        public XPCollection<Movie> Movies {
            get { return GetCollection<Movie>("Movies"); }
        }
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        public MovieCategoryPrice DvdPrice {
            get { return Prices.First(price => price.Format == MovieItemFormat.DVD); }
        }

        [Association("Category-Prices"), Aggregated]
        public XPCollection<MovieCategoryPrice> Prices {
            get { return GetCollection<MovieCategoryPrice>("Prices"); }
        }

        public bool IsDefault {
            get { return isDefault; }
            set { SetPropertyValue("IsDefault", ref isDefault, value); }
        }
        [VisibleInListView(false)]
        public List<ChartPriceData> ChartPrices {
            get {
                var ret = new List<ChartPriceData>();
                foreach (MovieCategoryPrice price in Prices) {
                    ret.Add(new ChartPriceData(price, -1));
                    for (int i = 0; i < 8; i++)
                        ret.Add(new ChartPriceData(price, i));
                }
                return ret;
            }
        }

        public MovieCategoryPrice GetPrice(MovieItemFormat movieItemFormat) {
            MovieCategoryPrice ret = null;
            Prices.Filter = CriteriaOperator.Parse("Format = ?", movieItemFormat);
            if (Prices.Count != 0) ret = Prices[0];
            Prices.Filter = null;
            return ret;
        }
    }
    [NonPersistent]
    public class ChartPriceData {
        readonly MovieCategoryPrice _movieCategoryPrice;
        readonly MovieItemFormat _format;
        readonly int _day;

        public ChartPriceData(MovieCategoryPrice movieCategoryPrice, int day) {
            _movieCategoryPrice = movieCategoryPrice;
            _price = day == -1 ? movieCategoryPrice.DefaultRentDays : GetMovieCategoryPriceDay(movieCategoryPrice, day);
            _day = day;
            _format = movieCategoryPrice.Format;
        }

        decimal GetMovieCategoryPriceDay(MovieCategoryPrice price, int day) {
            var ret = price.GetDaysRentPrice(day);
            return ret == 0 ? price.GetDaysRentPrice(0) : ret;
        }

        public string Type { get { return _format.ToString(); } }

        decimal _price;

        public decimal Price {
            get { return _price; }
            set {
                if (_day == -1)
                    _movieCategoryPrice.DefaultRentDays = (int)value;
                else
                    _movieCategoryPrice.SetDaysRentPrice(_day, value);
                _price = value;
            }
        }

        public int Day { get { return _day; } }
    }

    [FullPermission]
    [InitialData(BaseMembers = "oid|Oid", AllOwnMembers = true)]
    public class MovieCategoryPrice : VideoRentalBaseObject {
        MovieItemFormat format;
        int defaultRentDays;
        MovieCategory _category;
        static public int TermsRentCount = 7;
        readonly decimal[] daysRentPrice = new decimal[TermsRentCount + 1]; // daysRentPrice[0] - Late fee per day
        public MovieCategoryPrice(Session session) : base(session) { }

        public override void AfterConstruction() {
            base.AfterConstruction();
            Format = MovieItemFormat.DVD;
            DefaultRentDays = 1;
        }

        public decimal LateRentPrice { get { return GetDaysRentPrice(0); } set { SetDaysRentPrice(0, value); } }
        public decimal Days1RentPrice { get { return GetDaysRentPrice(1); } set { SetDaysRentPrice(1, value); } }
        public decimal Days2RentPrice { get { return GetDaysRentPrice(2); } set { SetDaysRentPrice(2, value); } }
        public decimal Days3RentPrice { get { return GetDaysRentPrice(3); } set { SetDaysRentPrice(3, value); } }
        public decimal Days4RentPrice { get { return GetDaysRentPrice(4); } set { SetDaysRentPrice(4, value); } }
        public decimal Days5RentPrice { get { return GetDaysRentPrice(5); } set { SetDaysRentPrice(5, value); } }
        public decimal Days6RentPrice { get { return GetDaysRentPrice(6); } set { SetDaysRentPrice(6, value); } }
        public decimal Days7RentPrice { get { return GetDaysRentPrice(7); } set { SetDaysRentPrice(7, value); } }
        public decimal GetDaysRentPrice(int index) { return daysRentPrice[index]; }
        public void SetDaysRentPrice(int index, decimal value) {
            if (index == 0) SetPropertyValue<decimal>("LateRentPrice", ref daysRentPrice[0], value);
            else SetPropertyValue<decimal>(string.Format("Days{0}RentPrice", index), ref daysRentPrice[index], value);
        }

        public int DefaultRentDays {
            get { return defaultRentDays; }
            set { SetPropertyValue<int>("DefaultRentDays", ref defaultRentDays, value); }
        }

        public MovieItemFormat Format {
            get { return format; }
            set { SetPropertyValue("Format", ref format, value); }
        }

        [Association("Category-Prices"), Indexed("Format", Unique = true)]
        public MovieCategory Category {
            get { return _category; }
            set { SetPropertyValue("Category", ref _category, value); }
        }

    }

}