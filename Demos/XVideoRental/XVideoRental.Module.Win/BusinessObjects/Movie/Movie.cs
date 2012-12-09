using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using System.Linq;
using Xpand.ExpressApp.Attributes;
using Xpand.ExpressApp.IO.Core;
using Xpand.ExpressApp.Security.Core;
using Xpand.ExpressApp.Win.SystemModule.ToolTip;
using Xpand.Utils.Helpers;

namespace XVideoRental.Module.Win.BusinessObjects.Movie {
    [Flags]
    public enum MovieGenre {
        None = 0, Action = 0x1, Adventure = 0x2, Animation = 0x4, Biography = 0x8, Comedy = 0x10, Crime = 0x20,
        Documentary = 0x40, Drama = 0x80, Family = 0x100, Fantasy = 0x200, History = 0x400, Horror = 0x800,
        Music = 0x1000, Musical = 0x2000, Mystery = 0x4000, Romance = 0x8000, SciFi = 0x10000, Sport = 0x20000,
        Thriller = 0x40000, War = 0x80000, Western = 0x100000
    }
    public enum MovieRating {
        [ImageName("GRating")]
        G,
        [ImageName("PGRating")]
        PG,
        [ImageName("PG13Rating")]
        PG13,
        [ImageName("RRating")]
        R,
        [ImageName("NC17Rating")]
        NC17
    }
    public enum MovieItemFormat {
        [ImageName("DVD")]
        DVD = 1,
        [ImageName("blueray")]
        BlueRay = 2,
        [ImageName("VideoCd")]
        VideoCD = 3
    }
    public enum MovieItemStatus { Active, Rented, Sold, Damaged, Lost }

    public class MovieToolTipController : ObjectToolTipController {
        const int MaxPhotoWidth = 120, MaxPhotoHeight = 120;
        public MovieToolTipController(Control parent) : base(parent) { }

        protected override void InitToolTipItem(ToolTipItem item) {
            var movie = ObjectSpace.FindObject<Movie>(CriteriaOperator.Parse("MovieTitle=?", EditObject));
            var photo = movie.Photo;
            if (photo != null)
                item.Image = photo.CreateImage(MaxPhotoWidth, MaxPhotoHeight);
            item.Text = movie.GetMovieInfoHtml();
        }
    }


    [DefaultClassOptions]
    [DefaultProperty("MovieTitle")]
    [FullPermission]
    [ImageName("BO_Movie")]
    [InitialData(AllOwnMembers = true, BaseMembers = "oid|Oid,Id|MovieId")]
    [CloneView(CloneViewType.ListView, "Movie_ListView_AdvBanded")]
    [CloneView(CloneViewType.ListView, "Movie_ListView_Layout")]
    [CloneView(CloneViewType.ListView, "Movie_ListView_Reports_Movie_Invetory")]
    public class Movie : VideoRentalBaseObject {
        string title;
        MovieGenre genre;
        MovieRating rating;
        DateTime? releaseDate;
        TimeSpan? runTime;
        bool isColor;
        Image photo;
        string tagline;
        string plot;
        string awards;
        string webSite;
        string aspectRatio;
        Language _language;
        MovieCategory _category;

        public Movie(Session session) : base(session) { }

        public Movie(Session session, string title)
            : this(session) {
            Title = title;
        }
        [PersistentAlias("Id")]
        public long MovieId {
            get { return (long)EvaluateAlias("MovieId"); }
        }

        public XPCollection<Rent.Rent> Rents {
            get {
                var ret = new XPCollection<Rent.Rent>(Session, false);
                foreach (Rent.Rent rent in Items.SelectMany(item => item.Rents)) {
                    ret.Add(rent);
                }
                return ret;
            }
        }

        public string Directors {
            get {
                var directors = Artists.Where(artist => artist.Line.IsDirector).Select(artist => artist.Artist.BirthName);
                return directors.Aggregate<string, string>(null, (current, director) => current + (director + ", ")).TrimEnd(", ".ToCharArray());
            }
        }

        [Size(SizeAttribute.Unlimited)]
        [RuleRequiredField]
        public string Title {
            get { return title; }
            set { SetPropertyValue("Title", ref title, value); }
        }
        [VisibleInListView(false)]
        public string MovieTitle { get { return string.Format("{0} ({1})", Title, ReleaseDate.HasValue ? ReleaseDate.Value.Year.ToString(CultureInfo.InvariantCulture) : "?"); } }
        [Size(SizeAttribute.Unlimited)]
        public string Tagline {
            get { return tagline; }
            set { SetPropertyValue("Tagline", ref tagline, value); }
        }
        [Size(SizeAttribute.Unlimited)]
        public string Plot {
            get { return plot; }
            set { SetPropertyValue("Plot", ref plot, value); }
        }
        public string Awards {
            get { return awards; }
            set { SetPropertyValue("Awards", ref awards, value); }
        }

        public MovieRating Rating {
            get { return rating; }
            set { SetPropertyValue("Rating", ref rating, value); }
        }
        public MovieGenre Genre {
            get { return genre; }
            set { SetPropertyValue("Genre", ref genre, value); }
        }
        public DateTime? ReleaseDate {
            get { return releaseDate; }
            set { SetPropertyValue("ReleaseDate", ref releaseDate, value); }
        }

        public TimeSpan? RunTime {
            get { return runTime; }
            set { SetPropertyValue("RunTime", ref runTime, value); }
        }
        public bool IsColor {
            get { return isColor; }
            set { SetPropertyValue("IsColor", ref isColor, value); }
        }
        [ValueConverter(typeof(ImageValueConverter))]
        public Image Photo {
            get { return photo; }
            set { SetPropertyValue("Photo", ref photo, value); }
        }
        public string WebSite {
            get { return webSite; }
            set { SetPropertyValue("WebSite", ref webSite, value); }
        }
        public string AspectRatio {
            get { return aspectRatio; }
            set { SetPropertyValue("AspectRatio", ref aspectRatio, value); }
        }

        [Association("Movies-Language")]
        public Language Language {
            get { return _language; }
            set { SetPropertyValue("Language", ref _language, value); }
        }

        [Association("Movie-MoviePictures"), Aggregated]
        public XPCollection<MoviePicture> Pictures {
            get { return GetCollection<MoviePicture>("Pictures"); }
        }

        [InitialData(DataProviderTableName = "CountryMovies", DataProviderQueryColumnName = "Movies", DataProviderResultColumnName = "Countries")]
        [Association("Movies-Countries")]
        public XPCollection<Country> Countries {
            get { return GetCollection<Country>("Countries"); }
        }

        [Association("Movie-MovieCompany"), Aggregated]
        public XPCollection<MovieCompany> Companies {
            get { return GetCollection<MovieCompany>("Companies"); }
        }

        [Association("Movie-MovieArtist"), Aggregated]
        public XPCollection<MovieArtist> Artists {
            get { return GetCollection<MovieArtist>("Artists"); }
        }

        [Association("Category-Movies")]
        public MovieCategory Category {
            get { return _category; }
            set { SetPropertyValue("Category", ref _category, value); }
        }

        [Association("Movie-Items"), Aggregated]
        public XPCollection<MovieItem> Items {
            get { return GetCollection<MovieItem>("Items"); }
        }

        public string GetMovieInfoHtml() {
            return string.Format("<b>{0}</b>\r\n<i>{2:D}</i>\r\r\n{1}", Title, Plot, ReleaseDate);
        }
    }
}