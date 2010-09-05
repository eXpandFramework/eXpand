using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;

namespace FeatureCenter.Module.WorldCreator.ExistentAssemblyMasterDetail
{
    public class AttributeRegistrator:Xpand.ExpressApp.Core.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type!= typeof(EAMDCustomer)) yield break;
            yield return new DisplayFeatureModelAttribute("EAMDCustomer_ListView", "ExistentAssemblyMasterDetailModelStore");
        }
    }
}
