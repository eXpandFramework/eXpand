using System;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Xpo;

namespace FeatureCenter.Module.RuntimeFields.OrphanedCollectionWithCode {
    public class CreateRuntimeOrphanedCollectionController : ViewController {
        public override void CustomizeTypesInfo(DevExpress.ExpressApp.DC.ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            var classInfo = XafTypesInfo.XpoTypeInfoSource.XPDictionary.GetClassInfo(typeof(Customer));

            if (classInfo.FindMember("OrderLinesFromCode") == null) {
                var attributes = new Attribute[] {new VisibleInListViewAttribute(false),new VisibleInLookupListViewAttribute(false),
                                                  new VisibleInDetailViewAttribute(false)};
                classInfo.CreateCollection("OrderLinesFromCode", typeof(OrderLine), "Order.Customer.Oid='@This.Oid'",attributes);
                typesInfo.RefreshInfo(typeof(Customer));
            }
        }
    }
}
