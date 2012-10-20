using System;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace FeatureCenter.Base {
    [NonPersistent]
    public abstract class OrderBase : BaseObject, IOrder {
        DateTime _orderDate;
        string _reference;
        float _total;

        protected OrderBase(Session session) :
            base(session) {
        }

        ICustomer IOrder.Customer {
            get { return GetCustomer(); }
            set { SetCustomer(value); }
        }

        protected abstract void SetCustomer(ICustomer customer);

        protected abstract ICustomer GetCustomer();


        
        [VisibleInListView(true)]
        public string Reference {
            get { return _reference; }
            set { SetPropertyValue("Reference", ref _reference, value); }
        }
        [VisibleInListView(true)]
        public DateTime OrderDate {
            get { return _orderDate; }
            set { SetPropertyValue("OrderDate", ref _orderDate, value); }
        }
        [VisibleInListView(true)]
        public float Total {
            get { return _total; }
            set { SetPropertyValue("Total", ref _total, value); }
        }
    }
}