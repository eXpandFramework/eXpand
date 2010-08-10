using System;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using FeatureCenter.Base;

namespace FeatureCenter.Module.WorldCreator.ExistentAssemblyMasterDetail {
    public class EAMDOrder:BaseObject,IOrder {
        public EAMDOrder(Session session) : base(session) {
        }


        ICustomer IOrder.Customer {
            get { return GetMemberValue("Customer") as ICustomer; }
            set { SetMemberValue("Customer",value); }
        }

        string IOrder.Reference {
            get { return GetMemberValue("Reference") as string; }
            set { SetMemberValue("Reference",value); }
        }

        DateTime IOrder.OrderDate {
            get { return (DateTime) GetMemberValue("OrderDate"); }
            set { SetMemberValue("OrderDate",value); }
        }

        float IOrder.Total {
            get { return (float) GetMemberValue("Total"); }
            set { SetMemberValue("Total",value); }
        }
    }
}