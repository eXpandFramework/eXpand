using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using FeatureCenter.Module.WorldCreator.DynamicAssemblyMasterDetail;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.WorldCreator.DynamicAssemblyCalculatedField {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        private const string DMDCustomerCalculatedField_ListView = "DMDCustomerCalculatedField_ListView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type.FullName != WorldCreatorUpdater.MasterDetailDynamicAssembly + "." + WorldCreatorUpdater.DMDCustomer) yield break;
            yield return new DisplayFeatureModelAttribute(DMDCustomerCalculatedField_ListView, "WCCalculatedFieldModelStore");
            yield return new CloneViewAttribute(CloneViewType.ListView, DMDCustomerCalculatedField_ListView);
            yield return new XpandNavigationItemAttribute("WorldCreator/Dynamic Assembly/Calculated Field",DMDCustomerCalculatedField_ListView);
        }
    }
}
