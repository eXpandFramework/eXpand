using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Win.ImportExport.DynamicAssemblyMasterDetail {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type.FullName == WorldCreatorUpdater.MasterDetailDynamicAssembly + "." + WorldCreatorUpdater.DMDCustomer) {
                yield return new XpandNavigationItemAttribute(Module.Captions.Importexport + "Dynamic assembly Master detail", "IODMDCustomer_ListView");
                yield return new DisplayFeatureModelAttribute("IODMDCustomer_ListView", "IOWC3LevelMasterDetail");
            }
        }
    }
}
