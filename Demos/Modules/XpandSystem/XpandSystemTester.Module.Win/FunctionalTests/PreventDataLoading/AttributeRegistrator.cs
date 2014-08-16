using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;
using XpandSystemTester.Module.FunctionalTests.PreventDataLoading;

namespace XpandSystemTester.Module.Win.FunctionalTests.PreventDataLoading {
    public class AttributeRegistrator:AttributeRegistrator<PreventDataLoadingObject> {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo){
            yield return
                new CloneViewAttribute(CloneViewType.ListView,
                    PreventDataLoadingObject.PreventDataLoadingGroupName + "/");
        }
    }
}
