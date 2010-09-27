using System;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Xpo;

namespace FeatureCenter.Module.RuntimeFields.CalculatedWithCode {
    public class CreateRuntimeCalculatedFieldController : ViewController {
        public override void CustomizeTypesInfo(DevExpress.ExpressApp.DC.ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            var classInfo = XafTypesInfo.XpoTypeInfoSource.XPDictionary.GetClassInfo(typeof(Customer));
            
            if (classInfo.FindMember("SumOfOrderTotals")==null) {
                var attributes = new Attribute[] {new PersistentAliasAttribute("Orders.Sum(Total)")};
                classInfo.CreateCalculabeMember("SumOfOrderTotals", typeof(float), attributes);
                typesInfo.RefreshInfo(typeof(Customer));
            }
        }
    }
}
