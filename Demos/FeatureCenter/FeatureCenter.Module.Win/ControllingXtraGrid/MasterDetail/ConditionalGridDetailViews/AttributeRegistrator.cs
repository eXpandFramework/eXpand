using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.MasterDetail.Logic;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.Win.ControllingXtraGrid.MasterDetail.ConditionalGridDetailViews {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type == typeof(WinCustomer)) {
                yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ViewMessage + " " + Captions.HeaderConditionalDetailGridViews, "1=1", "1=1",
                                                                     Captions.ViewMessageConditionalDetailGridViews, Position.Bottom) {
                                                                         Nesting = Nesting.Root,
                                                                         ViewType = ViewType.ListView, View = "ConditionalMasterDetailCustomer_ListView"
                                                                     };
                yield return new AdditionalViewControlsRuleAttribute(Module.Captions.Header + " " + Captions.HeaderConditionalDetailGridViews, "1=1", "1=1",
                                                                     Captions.HeaderConditionalDetailGridViews, Position.Top) { Nesting = Nesting.Root, ViewType = ViewType.ListView, View = "ConditionalMasterDetailCustomer_ListView" };
                yield return new MasterDetailAttribute("Customer_Orders_For_All_Other_Cities", "City!='Paris'", "ConditionalMasterDetailOrder_ListView", "Orders") { View = "ConditionalMasterDetailCustomer_ListView" };
                yield return new CloneViewAttribute(CloneViewType.ListView, "ConditionalMasterDetailCustomer_ListView");
                yield return new XpandNavigationItemAttribute("Controlling XtraGrid/Master Detail/Conditional Detail views", "ConditionalMasterDetailCustomer_ListView");
                yield return new DisplayFeatureModelAttribute("ConditionalMasterDetailCustomer_ListView", "ConditionalGridDetailViews");
            }
            if (typesInfo.Type == typeof(WinOrder)) {
                yield return new CloneViewAttribute(CloneViewType.ListView, "ConditionalMasterDetailOrderForParis_ListView");
                yield return new CloneViewAttribute(CloneViewType.ListView, "ConditionalMasterDetailOrder_ListView");
            }
        }
    }
}
