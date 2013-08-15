using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Win.PropertyEditors.NullAble {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(PENACustomer)) yield break;
            var xpandNavigationItemAttribute = new XpandNavigationItemAttribute(Module.Captions.PropertyEditors + "Null Able properties", "PENACustomer_ListView");
            yield return xpandNavigationItemAttribute;
            yield return new WhatsNewAttribute(new DateTime(2011, 1, 28), xpandNavigationItemAttribute);

        }
    }
}
