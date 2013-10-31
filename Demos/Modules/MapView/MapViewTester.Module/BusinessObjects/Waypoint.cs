using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo.Metadata;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;

namespace MapViewTester.Module.BusinessObjects
{

    [DefaultClassOptions]
    public class Waypoint : BaseObject
    {
        public Waypoint(Session session)
            : base(session)
        {
        }

        private string name;
        [Size(500)]
        public string Name
        {
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
        }

        private string comment;
        [Size(SizeAttribute.Unlimited)]
        public string Comment
        {
            get { return comment; }
            set { SetPropertyValue("Comment", ref comment, value); }
        }

        private decimal longtitude;
        [DbType("numeric(13,10)")]
        public decimal Longtitude
        {
            get { return longtitude; }
            set { SetPropertyValue("Longtitude", ref longtitude, value); }
        }

        private decimal latitude;
        [DbType("numeric(13,10)")]
        public decimal Latitude
        {
            get { return latitude; }
            set { SetPropertyValue("Latitude", ref latitude, value); }
        }
    }
}
