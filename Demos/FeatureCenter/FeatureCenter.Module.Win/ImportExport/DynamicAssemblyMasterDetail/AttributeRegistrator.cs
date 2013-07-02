using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Win.ImportExport.DynamicAssemblyMasterDetail {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type.FullName == WorldCreatorUpdater.MasterDetailDynamicAssembly + "." + WorldCreatorUpdater.DMDCustomer) {
                yield return new XpandNavigationItemAttribute(Module.Captions.Importexport + "Dynamic assembly Master detail", "IOMasterDetailDynamicAssembly.IODMDCustomer_ListView");
                yield return new DisplayFeatureModelAttribute("IOMasterDetailDynamicAssembly.IODMDCustomer_ListView", "IOWC3LevelMasterDetail");
            }
        }
    }
}
