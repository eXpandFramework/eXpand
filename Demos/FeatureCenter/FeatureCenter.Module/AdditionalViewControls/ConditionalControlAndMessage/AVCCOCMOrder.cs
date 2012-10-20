using DevExpress.Xpo;
using FeatureCenter.Base;

namespace FeatureCenter.Module.AdditionalViewControls.ConditionalControlAndMessage {
    public class AVCCOCMOrder : OrderBase {
        AVCCOCMCustomer _customer;

        public AVCCOCMOrder(Session session) : base(session) {
        }

        [Association("AVCCOCMCustomer-AVCCOCMOrders")]
        public AVCCOCMCustomer Customer {
            get { return _customer; }
            set { SetPropertyValue("Customer", ref _customer, value); }
        }

        protected override void SetCustomer(ICustomer customer) {
            Customer = (AVCCOCMCustomer) customer;
        }

        protected override ICustomer GetCustomer() {
            return Customer;
        }

    }
}