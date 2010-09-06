using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Miscellaneous.LinqQuery
{
    public class AttributeRegistrator:Xpand.ExpressApp.Core.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderLinqQuery, "1=1", "1=1", Captions.ViewMessageLinqQuery, Position.Bottom) { ViewType = ViewType.ListView, View = "Customer_ListView_EmployeesLinq_Linq" };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderLinqQuery, "1=1", "1=1", Captions.HeaderLinqQuery, Position.Top) { ViewType = ViewType.ListView, View = "Customer_ListView_EmployeesLinq_Linq" };
            yield return new XpandNavigationItemAttribute(Captions.Miscellaneous + "LinqQuery", "Customer_ListView_EmployeesLinq_Linq");
            yield return new CloneViewAttribute(CloneViewType.ListView, "LinqQuery_ListView");
        }
    }
}
