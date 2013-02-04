using System;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp;
using Xpand.Xpo;

namespace FeatureCenter.Module.RuntimeFields.CalculatedWithCode {
    public class CreateRuntimeCalculatedFieldController : ViewController {
        public override void CustomizeTypesInfo(DevExpress.ExpressApp.DC.ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            var classInfo = XpandModuleBase.Dictiorary.GetClassInfo(typeof(Customer));

            if (classInfo.FindMember("SumOfOrderTotals") == null) {
                var xpandCalcMemberInfo = classInfo.CreateCalculabeMember("SumOfOrderTotals", typeof(float), "Orders.Sum(Total)");
                var attributes = new Attribute[] {new VisibleInListViewAttribute(false),new VisibleInLookupListViewAttribute(false),
                                                  new VisibleInDetailViewAttribute(false)};
                foreach (var attribute in attributes) {
                    xpandCalcMemberInfo.AddAttribute(attribute);
                }

                typesInfo.RefreshInfo(typeof(Customer));
            }
        }
    }
}
