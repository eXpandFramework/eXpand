using System.Drawing;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using XXXVideoRental.Module.Win.DatabaseUpdate;
using Xpand.ExpressApp.IO.Core;
using Xpand.ExpressApp.Security.Core;

namespace XXXVideoRental.Module.Win.BusinessObjects.Movie {
    [PermissionBehavior(PermissionBehavior.ReadOnlyAccess)]
    public class ArtistPicture : Picture {
        Artist artist;

        public ArtistPicture(Session session) : base(session) { }
        public ArtistPicture(Artist artist, Image picture)
            : this(artist.Session) {
            Artist = artist;
            Image = picture;
        }
        public ArtistPicture(Artist artist, Image picture, string description)
            : this(artist, picture) {
            Description = description;
        }
        [Association("Artist-ArtistPictures")]
        [RuleRequiredField]
        public Artist Artist {
            get { return artist; }
            set { SetPropertyValue("Artist", ref artist, value); }
        }
    }

    [FullPermission]
    [InitialData(BaseMembers = "Image,Description,oid|Oid", Name = "vMoviePicture", AllOwnMembers = true)]
    public class MoviePicture : Picture {
        Movie movie;

        public MoviePicture(Session session) : base(session) { }
        public MoviePicture(Movie movie, Image picture)
            : this(movie.Session) {
            Movie = movie;
            Image = picture;
        }
        public MoviePicture(Movie movie, Image picture, string description)
            : this(movie, picture) {
            Description = description;
        }

        [Association("Movie-MoviePictures")]
        [RuleRequiredField]
        public Movie Movie {
            get { return movie; }
            set { SetPropertyValue("Movie", ref movie, value); }
        }
    }
    [NonPersistent]
    public abstract class Picture : VideoRentalBaseObject {
        Image image;
        string description;

        protected Picture(Session session) : base(session) { }
        [ValueConverter(typeof(ImageValueConverter))]
        public Image Image {
            get { return image; }
            set { SetPropertyValue("Image", ref image, value); }
        }
        [Size(200)]
        public string Description {
            get { return description; }
            set { SetPropertyValue("Description", ref description, value); }
        }
    }

}