using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.PivotChart.PivotedProperty;
using FeatureCenter.Base;
using Xpand.ExpressApp.Attributes;
using Xpand.Xpo.Collections;

namespace FeatureCenter.Module.PivotChart.ConnectWithCustomer {
    [XpandNavigationItem("PivotChart/Connecting with customer", "CAWCustomer_DetailView")]
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderConnectWithCustomer, "1=1", "1=1", Captions.ViewMessageConnectWithCustomer, Position.Bottom, ViewType = ViewType.DetailView)]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderConnectWithCustomer, "1=1", "1=1", Captions.HeaderConnectWithCustomer, Position.Top, ViewType = ViewType.DetailView)]
    [DisplayFeatureModel("CAWCustomer_DetailView", "ConnectWithCustomer")]
    public class CAWCustomer : CustomerBase {
        public CAWCustomer(Session session)
            : base(session) {
        }

        [Association("CWCustomer-Orders")]
        public XPCollection<CAWOrder> Orders {
            get { return GetCollection<CAWOrder>("Orders"); }
        }

        [PivotedProperty("OrderLines", "Name='Controlling Grid Settings'")]
        public Analysis ControllingGridSettingsAnalysis { get; set; }

        [PivotedProperty("OrderLines", "Name='HideChart'")]
        public Analysis HideChartAnalysis { get; set; }
        [PivotedProperty("OrderLines", "Name='HidePivot'")]
        public Analysis HidePivotAnalysis { get; set; }
        [PivotedProperty("OrderLines", "Name='InPlaceEdit'")]
        public Analysis InPlaceEditAnalysis { get; set; }
        [PivotedProperty("OrderLines", "Name='PivotGroupInterval'")]
        public Analysis InPlacePivotGroupInterval { get; set; }
        [PivotedSort("UnitPrice", SortDirection.Descending, "UnitPrice")]
        [PivotedProperty("OrderLines", "Name='Custom Sort'")]
        public Analysis CustomSortedAnalysis { get; set; }
        [Browsable(false)]
        public XPCollection<CAWOrderLine> OrderLines {
            get {
                return new XpandXPCollection<CAWOrderLine>(Session, line => line.Order.Customer.Oid == Oid);
            }
        }
    }
}
