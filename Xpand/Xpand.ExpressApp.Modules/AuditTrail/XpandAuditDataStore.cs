using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.AuditTrail;
using DevExpress.Xpo;
using Xpand.ExpressApp.AuditTrail.BusinessObjects;

namespace Xpand.ExpressApp.AuditTrail{
    public class XpandAuditDataStore : AuditDataStore<XpandAuditDataItemPersistent, XpandAuditedObjectWeakReference> {
        protected override string GetDefaultStringRepresentation(object value){
            if (value == null) {
                return NullValueString;
            }
            if (value is XPWeakReference) {
                if (!(value as XPWeakReference).IsAlive) {
                    return NullValueString;
                }
                return (value as XPWeakReference).Target.ToString();
            }
            ITypeInfo ti = XafTypesInfo.Instance.FindTypeInfo(value.GetType());
            IMemberInfo defaultMember = (ti != null) ? ti.DefaultMember : null;
            string result;
            if (defaultMember != null){
                object memberValue = defaultMember.GetValue(value);
                result = memberValue == null ? NullValueString : memberValue.ToString();
            }
            else {
                result = value.ToString();
            }
            if (!(value is string) && result.Length > MaxLengthOfValue) {
                result = BlobDataString;
            }
            return result;

        }
    }
}