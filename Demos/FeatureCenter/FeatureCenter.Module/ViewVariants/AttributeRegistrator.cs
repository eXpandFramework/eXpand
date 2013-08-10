using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.ViewVariants {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            var list = new List<string> { "ViewVariants_ListView", "Hong Kong Customers", "London Customers", "Paris Customers", "New York Customers" };
            foreach (string t in list) {
                yield return new DisplayFeatureModelAttribute(t, "ViewVariants");
            }
            yield return new CloneViewAttribute(CloneViewType.ListView, "ViewVariants_ListView");
            yield return new XpandNavigationItemAttribute("View Variants", "ViewVariants_ListView");
            yield return new DisplayFeatureModelAttribute("ViewVariants_ListView", "ViewVariants");
        }
    }
}
