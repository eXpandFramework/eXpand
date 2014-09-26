using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.Win.WorldCreator.DynamicAssemblyCalculatedField {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        public const string MasterDetailDynamicAssembly = "MasterDetailDynamicAssembly";
        public const string DMDCustomer = "DMDCustomer";
        private const string DMDCustomerCalculatedField_ListView = "DMDCustomerCalculatedField_ListView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type.FullName != MasterDetailDynamicAssembly + "." + DMDCustomer) yield break;
            yield return new DisplayFeatureModelAttribute(DMDCustomerCalculatedField_ListView, "WCCalculatedFieldModelStore");
            yield return new CloneViewAttribute(CloneViewType.ListView, DMDCustomerCalculatedField_ListView);
            yield return new XpandNavigationItemAttribute("WorldCreator/Dynamic Assembly/Calculated Field",DMDCustomerCalculatedField_ListView);
        }
    }
}
