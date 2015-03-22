using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapViewTester.Module.BusinessObjects
{
    [DefaultClassOptions]
    public class CustomerGroup : BaseObject
    {
        public CustomerGroup(Session session) : base(session)
        {

        }


        private string name;
        public string Name
        {
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
        }
        [Association]
        public XPCollection<Customer> Customers { get { return GetCollection<Customer>("Customers"); } }

        [Association]
        public XPCollection<Waypoint> WayPoints { get { return GetCollection<Waypoint>("WayPoints"); } }

    }
}
