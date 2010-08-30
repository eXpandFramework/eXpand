using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.ViewVariants
{
    public class AttributeRegistrator:Module.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type!=typeof(Customer))yield break;
            var list=new List<string> {"ViewVariants_ListView","Hong Kong Customers","London Customers","Paris Customers","New York Customers"};
            foreach (string t in list) {
                yield return new DisplayFeatureModelAttribute("ViewVariants_ListView", "ViewVariants");
            }
            yield return new CloneViewAttribute(CloneViewType.ListView, "ViewVariants_ListView");
            yield return new NavigationItemAttribute("View Variants", "ViewVariants_ListView");
            yield return new DisplayFeatureModelAttribute("ViewVariants_ListView", "ViewVariants");
        }
    }
}
