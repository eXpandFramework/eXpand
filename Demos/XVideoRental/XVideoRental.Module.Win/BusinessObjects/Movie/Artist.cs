using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using XVideoRental.Module.Win.DatabaseUpdate;
using Xpand.ExpressApp.IO.Core;
using Xpand.ExpressApp.Security.Core;

namespace XVideoRental.Module.Win.BusinessObjects.Movie {
    [FullPermission]
    [InitialData(BaseMembers = "oid|Oid", AllOwnMembers = true)]
    [RuleCombinationOfPropertiesIsUnique(DefaultContexts.Save, "Movie,Artist")]
    public class MovieArtist : VideoRentalBaseObject {
        Artist artist;
        Movie movie;
        MovieArtistLine line;
        string description;

        public MovieArtist(Session session) : base(session) { }
        [RuleRequiredField]
        [Association("Movie-MovieArtist")]
        public Movie Movie {
            get { return movie; }
            set { SetPropertyValue("Movie", ref movie, value); }
        }
        [Association("Artist-MovieArtist")]
        [RuleRequiredField]
        public Artist Artist {
            get { return artist; }
            set { SetPropertyValue("Artist", ref artist, value); }
        }
        [Size(SizeAttribute.Unlimited)]
        public string Description {
            get { return description; }
            set { SetPropertyValue("Description", ref description, value); }
        }
        public MovieArtistLine Line {
            get { return line; }
            set { SetPropertyValue("Line", ref line, value); }
        }
    }

    [FullPermission]
    [InitialData(BaseMembers = "oid|Oid,Birthday|BirthDate,FirstName,LastName,NickName,Id|ArtistId", AllOwnMembers = true, Name = "vArtist")]
    [ImageName("BO_Actor")]
    public class Artist : VideoPerson {
        string birthName;
        Country birthCountry;
        string birthLocation;
        string biography;
        string nickName;
        string link;

        public Artist(Session session) : base(session) { }

        [PersistentAlias("Id")]
        public long ArtistId {
            get { return (long)EvaluateAlias("ArtistId"); }
        }

        public string BirthName {
            get { return birthName; }
            set { SetPropertyValue("BirthName", ref birthName, value); }
        }

        public Country BirthCountry {
            get { return birthCountry; }
            set { SetPropertyValue("BirthCountry", ref birthCountry, value); }
        }

        public string BirthLocation {
            get { return birthLocation; }
            set { SetPropertyValue("BirthLocation", ref birthLocation, value); }
        }

        [Size(SizeAttribute.Unlimited)]
        public string Biography {
            get { return biography; }
            set { SetPropertyValue("Biography", ref biography, value); }
        }

        public string NickName {
            get { return nickName; }
            set { SetPropertyValue("NickName", ref nickName, value); }
        }

        public string Link {
            get { return link; }
            set { SetPropertyValue("Link", ref link, value); }
        }


        [Association("Artist-MovieArtist"), Aggregated]
        public XPCollection<MovieArtist> Movies {
            get { return GetCollection<MovieArtist>("Movies"); }
        }
        [Association("Artist-ArtistPictures"), Aggregated]
        public XPCollection<ArtistPicture> Pictures { get { return GetCollection<ArtistPicture>("Pictures"); } }

    }
    [PermissionBehavior(PermissionBehavior.ReadOnlyAccess)]
    [InitialData(BaseMembers = "oid|Oid", AllOwnMembers = true, ThrowIfColumnNotFound = false)]
    public class MovieArtistLine : VideoRentalBaseObject {
        public const string Director = "Director";
        string name;

        public MovieArtistLine(Session session) : base(session) { }
        [Indexed(Unique = true)]
        [RuleRequiredField]
        public string Name {
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
        }
        private bool _isDirector;

        public bool IsDirector {
            get { return _isDirector; }
            set { SetPropertyValue("IsDirector", ref _isDirector, value); }
        }
        protected override void OnSaving() {
            base.OnSaving();
            if (Session.IsNewObject(this) && Name == Director) {
                IsDirector = true;
            }
        }
    }

}