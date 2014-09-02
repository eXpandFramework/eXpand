using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.Attributes;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.Win.ApplicationDifferences.ExternalApplication {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        private const string ExternalApplicationDetailView = "ExternalApplication_DetailView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
#if EASYTEST
            yield break;
            ;
#endif
            if (typesInfo.Type != typeof(ModelDifferenceObject)) yield break;
            yield return new DisplayFeatureModelAttribute(ExternalApplicationDetailView, "ExternalApplication");
            yield return new CloneViewAttribute(CloneViewType.DetailView, ExternalApplicationDetailView);
            yield return new XpandNavigationItemAttribute("Application Differences/External Application",
                    ExternalApplicationDetailView,"@ExternalApplicationKey");
        }
    }
}
