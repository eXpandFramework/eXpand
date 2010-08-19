using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;
using eXpand.ExpressApp.MasterDetail.Logic;

namespace FeatureCenter.Module.Win.XtraGrid.MasterDetail.ConditionalDetailViews
{
    public class AttributeRegistrator : Module.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type==typeof(WinCustomer)) {
                yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ViewMessage + " " + Captions.HeaderConditionalDetailGridViews, "1=1", "1=1",
                                                                     Captions.ViewMessageConditionalDetailGridViews, Position.Bottom){Nesting = Nesting.Root,
                                                                                                                                      ViewType = ViewType.ListView, View = "ConditionalMasterDetailCustomer_ListView"};
                yield return new AdditionalViewControlsRuleAttribute(Module.Captions.Header + " " + Captions.HeaderConditionalDetailGridViews, "1=1", "1=1",
                                                                     Captions.HeaderConditionalDetailGridViews, Position.Top){Nesting = Nesting.Root, ViewType = ViewType.ListView, View = "ConditionalMasterDetailCustomer_ListView"};
                yield return new MasterDetailAttribute("Customer_Orders_For_All_Other_Cities", "1=1", "ConditionalMasterDetailOrder_ListView", "Orders") { View = "ConditionalMasterDetailCustomer_ListView" };
                yield return new CloneViewAttribute(CloneViewType.ListView, "ConditionalMasterDetailCustomer_ListView");
                yield return new NavigationItemAttribute("Controlling XtraGrid/Master Detail/Conditional Detail views", "ConditionalMasterDetailCustomer_ListView");
                yield return new DisplayFeatureModelAttribute("ConditionalMasterDetailCustomer_ListView", "ConditionalDetailViews");
            }
            if (typesInfo.Type==typeof(WinOrder)) {
                yield return new CloneViewAttribute(CloneViewType.ListView, "ConditionalMasterDetailOrderForParis_ListView");
                yield return new CloneViewAttribute(CloneViewType.ListView, "ConditionalMasterDetailOrder_ListView");
            }
        }
    }
}
