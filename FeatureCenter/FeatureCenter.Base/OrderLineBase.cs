using System;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace FeatureCenter.Base {
    [NonPersistent]
    public abstract class OrderLineBase : BaseObject, IOrderLine {
        string _article;
        DateTime _orderLineDate;
        float _quantity;
        float _unitPrice;

        protected OrderLineBase(Session session) :
            base(session) {
        }

        IOrder IOrderLine.Order {
            get { return GetOrder(); }
            set { SetOrder(value); }
        }

        protected abstract void SetOrder(IOrder order);
         
        protected abstract IOrder GetOrder();
        [VisibleInListView(true)]
        public string Article {
            get { return _article; }
            set { SetPropertyValue("Article", ref _article, value); }
        }
        [VisibleInListView(true)]
        public float Quantity {
            get { return _quantity; }
            set { SetPropertyValue("Quantity", ref _quantity, value); }
        }
        [VisibleInListView(true)]
        public float UnitPrice {
            get { return _unitPrice; }
            set { SetPropertyValue("PPCustomer", ref _unitPrice, value); }
        }
        [VisibleInListView(true)]
        public DateTime OrderLineDate {
            get { return _orderLineDate; }
            set { SetPropertyValue("OrderLineDate", ref _orderLineDate, value); }
        }

        [Persistent]
        [VisibleInListView(true)]
        public float TotalPrice {
            get { return _quantity*_unitPrice; }
        }
    }
}