using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.MasterDetail.Logic;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.Win.ControllingXtraGrid.MasterDetail.AutoExpandNewRow {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type == typeof(WinCustomer)) {
                yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ViewMessage + " " + Captions.HeaderAutoExpandNewRow, "1=1", "1=1",
                                                                     Captions.ViewMessageAutoExpandNewRow, Position.Bottom) { Nesting = Nesting.Root, ViewType = ViewType.ListView, View = "AutoExpandNewRowCustomer_ListView" };
                yield return new AdditionalViewControlsRuleAttribute(Module.Captions.Header + " " + Captions.HeaderAutoExpandNewRow, "1=1", "1=1",
                                                                     Captions.HeaderAutoExpandNewRow, Position.Top) { Nesting = Nesting.Root, ViewType = ViewType.ListView, View = "AutoExpandNewRowCustomer_ListView" };
                yield return new MasterDetailAttribute("AutoExpandNewRowCustomer_Orders", "1=1", "AutoExpandNewRowOrder_ListView", "Orders") { View = "AutoExpandNewRowCustomer_ListView" };
                yield return new CloneViewAttribute(CloneViewType.ListView, "AutoExpandNewRowCustomer_ListView");
                yield return new XpandNavigationItemAttribute("Controlling XtraGrid/Master Detail/Auto Expand new row", "AutoExpandNewRowCustomer_ListView");
                yield return new DisplayFeatureModelAttribute("AutoExpandNewRowCustomer_ListView", "AutoExpandNewRow");
            }
            if (typesInfo.Type == typeof(WinOrder)) {
                yield return new CloneViewAttribute(CloneViewType.ListView, "AutoExpandNewRowOrder_ListView");
            }
        }
    }
}
