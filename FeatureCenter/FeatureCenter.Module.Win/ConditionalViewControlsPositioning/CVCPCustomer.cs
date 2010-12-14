using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using FeatureCenter.Base;

namespace FeatureCenter.Module.AdditionalViewControls.ConditionalViewControlsPositioning
{
//    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderConditionalViewControlsPositioning, "1=1", "1=1", Captions.ViewMessageAdditionalViewControls, Position.Bottom, ViewType = ViewType.ListView)]
//    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderConditionalViewControlsPositioning, "1=1", "1=1", Captions.HeaderConditionalViewControlsPositioning, Position.Top)]
//    [AdditionalViewControlsRule(Captions.ConditionalViewControlsPositioningForCustomerName, "1=1", "1=1", null, Position.DetailViewItem, MessageProperty = "NameWarning", ExecutionContextGroup = "Warning")]
//    [AdditionalViewControlsRule(Captions.ConditionalViewControlsPositioningForCustomerCity, "1=1", "1=1", null, Position.DetailViewItem, MessageProperty = "CityWarning", ExecutionContextGroup = "Warning")]
    public class CVCPCustomer:CustomerBase
    {
        public CVCPCustomer(Session session) : base(session) {
        }

        [Browsable(false)]
        public string NameWarning
        {
        	get {
        	    return (Name+"").Length>20 ? "Who gave him this name!!! "+Name : null;
        	}
        }
        [Browsable(false)]
        public string CityWarning
        {
        	get {
        	    return (City+"").Length<3 ? "Last week I was staying at "+City : null;
        	}
        }
        
    }
}
