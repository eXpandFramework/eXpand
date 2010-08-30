using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;
using eXpand.ExpressApp.MasterDetail.Logic;

namespace FeatureCenter.Module.Win.ControllingXtraGrid.MasterDetail.AtAnyLevel
{
    public class AttributeRegistrator : Module.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type==typeof(WinCustomer))
            {
                yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ViewMessage + " " + Captions.HeaderMasterDetail + "1", "1=1", "1=1",
                                                                     Captions.ViewMessageMasterDetail1, Position.Bottom){ViewType = ViewType.ListView, View = "MasterDetailAtAnyLevelCustomer_ListView"};
                yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ViewMessage + " " + Captions.HeaderMasterDetail + "2", "1=1", "1=1",
                                                                     Captions.ViewMessageMasterDetail2, Position.Bottom){ViewType = ViewType.ListView, View = "MasterDetailAtAnyLevelCustomer_ListView"};
                yield return new AdditionalViewControlsRuleAttribute(Module.Captions.Header + " " + Captions.HeaderMasterDetail, "1=1", "1=1",
                                                                     Captions.HeaderMasterDetail, Position.Top){View = "MasterDetailAtAnyLevelCustomer_ListView"};
                yield return new MasterDetailAttribute("AtAnyLevelCustomer_Orders", "1=1", "MasterDetailAtAnyLevelOrder_ListView", "Orders") { View = "MasterDetailAtAnyLevelCustomer_ListView" };
                yield return new CloneViewAttribute(CloneViewType.ListView, "MasterDetailAtAnyLevelCustomer_ListView");
                yield return new NavigationItemAttribute("Controlling XtraGrid/Master Detail/At any level", "MasterDetailAtAnyLevelCustomer_ListView");
                yield return new DisplayFeatureModelAttribute("MasterDetailAtAnyLevelCustomer_ListView", "AtAnyLevel");
            }
            if (typesInfo.Type==typeof(WinOrder)) {
                yield return new CloneViewAttribute(CloneViewType.ListView, "MasterDetailAtAnyLevelOrder_ListView");
                yield return new MasterDetailAttribute("AtAnyLevelOrder_OrderLines", "1=1", "MasterDetailAtAnyLevelOrderLine_ListView", "OrderLines") { View = "MasterDetailAtAnyLevelOrder_ListView" };
            }
            if (typesInfo.Type==typeof(WinOrderLine)) {
                yield return new CloneViewAttribute(CloneViewType.ListView, "MasterDetailAtAnyLevelOrderLine_ListView");
            }
        }
    }
}
