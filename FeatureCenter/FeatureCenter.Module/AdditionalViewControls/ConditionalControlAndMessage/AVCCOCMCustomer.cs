using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using FeatureCenter.Base;

namespace FeatureCenter.Module.AdditionalViewControls.ConditionalControlAndMessage
{
//    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderConditionalControlAndMessage, "1=1", "1=1", Captions.ViewMessageAdditionalViewControls, Position.Bottom, ViewType = ViewType.ListView)]
//    [AdditionalViewControlsRule(Captions.ConditionalAdditionalViewControlAndMessage, "Orders.Count>9", "1=0",
//        null, Position.Top, ViewType = ViewType.ListView,MessageProperty = "Message")]
//    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderConditionalControlAndMessage, "1=1", "1=1", Captions.HeaderConditionalControlAndMessage, Position.Top)]
    public class AVCCOCMCustomer:CustomerBase
    {
        public AVCCOCMCustomer(Session session) : base(session) {
        }


        [Association("AVCCOCMCustomer-AVCCOCMOrders")]
        public XPCollection<AVCCOCMOrder> Orders {
            get { return GetCollection<AVCCOCMOrder>("Orders"); }
        }

        [Browsable(false)]
        public string Message
        {
            get { return "Customer "+Name+" has less than "+Orders.Count; }
            
        }
        
    }
}
