using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace MainDemo.Module.BusinessObjects {
    public class Location : BaseObject, IMapsMarker {
        private Contact contact;
        private double latitude;
        private double longitude;

        public Location(Session session) :
            base(session) {
        }

        public override string ToString() {
            string latitudePrefix = Latitude > 0 ? "N" : "S";
            string longitudePrefix =  Longitude > 0 ? "E" : "W";
            return string.Format("{0}{1:0.###}, {2}{3:0.###}", latitudePrefix, Math.Abs(Latitude), longitudePrefix, Math.Abs(Longitude));
        }

        [Browsable(false)]
        public Contact Contact {
            get {
                return contact;
            }
            set {
                SetPropertyValue("Contact", ref contact, value);
            }
        }

        [PersistentAlias("Contact.FullName")]
        public string Title {
            get { return Convert.ToString(EvaluateAlias("Title")); }
        }

        public double Latitude {
            get {
                return latitude;
            }
            set {
                SetPropertyValue("Latitude", ref latitude, value);
            }
        }

        public double Longitude {
            get {
                return longitude;
            }
            set {
                SetPropertyValue("Longitude", ref longitude, value);
            }
        }
    }
}
