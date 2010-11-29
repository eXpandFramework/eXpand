using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace FeatureCenter.Module.Win.ApplicationDifferences.ExternalApplication {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(ModelDifferenceObject)) yield break;
            yield return new DisplayFeatureModelAttribute("ExternalApplication_DetailView", "ExternalApplication");
        }
    }
}
