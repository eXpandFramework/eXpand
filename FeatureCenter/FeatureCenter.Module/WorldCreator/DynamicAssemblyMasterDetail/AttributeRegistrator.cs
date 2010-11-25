using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.WorldCreator.DynamicAssemblyMasterDetail {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type.FullName != WorldCreatorUpdater.MasterDetailDynamicAssembly + "." + WorldCreatorUpdater.DMDCustomer) yield break;
            yield return new DisplayFeatureModelAttribute("DMDCustomer_ListView", "WC3LevelMasterDetailModelStore");
            yield return new XpandNavigationItemAttribute("WorldCreator/Dynamic Assembly/Master detail", "DMDCustomer_ListView");
        }
    }
}
