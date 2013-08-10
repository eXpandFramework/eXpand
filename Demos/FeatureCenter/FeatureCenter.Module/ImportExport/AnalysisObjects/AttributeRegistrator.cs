using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.ImportExport.AnalysisObjects {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        private const string ViewId = "IOAnalysis_ListView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type == typeof(DevExpress.Persistent.BaseImpl.Analysis)) {
                yield return new CloneViewAttribute(CloneViewType.ListView, ViewId);
                yield return new XpandNavigationItemAttribute(Captions.Importexport + "Analysis", ViewId);
                yield return new DisplayFeatureModelAttribute(ViewId, "Analysis");
            }

        }
    }
}
