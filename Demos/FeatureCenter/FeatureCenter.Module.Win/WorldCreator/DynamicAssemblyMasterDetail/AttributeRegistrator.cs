using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Win.WorldCreator.DynamicAssemblyMasterDetail {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type.FullName != DynamicAssemblyCalculatedField.AttributeRegistrator.MasterDetailDynamicAssembly + "." + DynamicAssemblyCalculatedField.AttributeRegistrator.DMDCustomer) yield break;
            yield return new DisplayFeatureModelAttribute("DMDCustomer_ListView", "WC3LevelMasterDetailModelStore");
            yield return new XpandNavigationItemAttribute("WorldCreator/Dynamic Assembly/Master detail", "DMDCustomer_ListView");
        }
    }
}
