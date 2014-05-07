using DevExpress.Data.Filtering;
using DevExpress.Persistent.AuditTrail;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.AuditTrail.BusinessObjects{
    [System.ComponentModel.DisplayName("AuditedObjectWeakReference")]
    public class XpandAuditedObjectWeakReference : BaseAuditedObjectWeakReference{
        public XpandAuditedObjectWeakReference(Session session) : base(session){
        }

        public XpandAuditedObjectWeakReference(Session session, object target) : base(session, target){
        }

        [Association("XpandAuditedObjectWeakReference-XpandAuditDataItemPersistent", typeof (XpandAuditDataItemPersistent))]
        public XPCollection<XpandAuditDataItemPersistent> AuditDataItems{
            get { return GetCollection<XpandAuditDataItemPersistent>("AuditDataItems"); }
        }

        [PersistentAlias("TargetType_.TypeName")]
        public string TargetTypeName {
            get { return EvaluateAlias("TargetTypeName") as string; }
        }

        public static XPCollection<XpandAuditDataItemPersistent> GetAuditTrail(Session session, object obj){
            var auditObjectWr = session.FindObject<XpandAuditedObjectWeakReference>(
                new GroupOperator(
                    new BinaryOperator("TargetType", session.GetObjectType(obj)),
                    new BinaryOperator("TargetKey", KeyToString(session.GetKeyValue(obj)))
                    ));
            if (auditObjectWr != null){
                auditObjectWr.AuditDataItems.BindingBehavior = CollectionBindingBehavior.AllowNone;
                return auditObjectWr.AuditDataItems;
            }
            return null;
        }
    }
}