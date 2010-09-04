using System;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using FeatureCenter.Base;
using Xpand.ExpressApp.Attributes;
using IQueryable = System.Linq.IQueryable;
using System.Linq;

namespace FeatureCenter.Module
{
   
    public class Customer : CustomerBase {
        public Customer(Session session) : base(session) {
        }

        [Association("Customer-Orders")]
        public XPCollection<Order> Orders {
            get { return GetCollection<Order>("Orders"); }
        }
        private DateTime _birthDate;
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public DateTime BirthDate {
            get { return _birthDate; }
            set { SetPropertyValue("BirthDate", ref _birthDate, value); }
        }
        [Browsable(false)]
        public string ConditionalControlAndMessage{
            get { return "Customer " + Name + " has less than " + Orders.Count; }
        }

        [Browsable(false)]
        public string NameWarning {
            get { return (Name + "").Length > 20 ? "Who gave him this name!!! " + Name : null; }
        }

        [Browsable(false)]
        public string CityWarning {
            get { return (City + "").Length < 3 ? "Last week I was staying at " + City : null; }
        }
        
        [CustomQueryProperties("DisplayableProperties", "Name_City;Orders_Last_OrderDate")]
        public static IQueryable EmployeesLinq(Session session)
        {
            return new XPQuery<Customer>(session).Select(customer =>
                                                         new
                                                         {
                                                             Name_City = customer.Name + " " + customer.City,
                                                             Orders_Last_OrderDate = customer.Orders.Max(order => order.OrderDate)
                                                         });
        }
        
    }
}