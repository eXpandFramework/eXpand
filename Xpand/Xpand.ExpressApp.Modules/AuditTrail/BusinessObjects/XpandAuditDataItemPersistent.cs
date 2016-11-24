using System;
using DevExpress.Persistent.AuditTrail;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General.CustomAttributes;
using Xpand.Persistent.Base.General.Model;
using Xpand.Xpo;

namespace Xpand.ExpressApp.AuditTrail.BusinessObjects{
    [ImageName("BO_Audit_ChangeHistory")]
    [CloneView(CloneViewType.ListView, "AuditDataItemPersistent_Pending_ListView")]
    [CloneView(CloneViewType.ListView, "AuditDataItemPersistent_Approved_ListView")]
    [System.ComponentModel.DisplayName("AuditDataItemPersistent")]
    public class XpandAuditDataItemPersistent : XpandCustomObject, IAuditDataItemPersistent<XpandAuditedObjectWeakReference>{
        private bool _pending;
        private XpandAuditedObjectWeakReference _auditedObject;
        private DateTime _modifiedOn;
        private XPWeakReference _newObject;
        private XPWeakReference _oldObject;
        private string _operationType;
        private string _propertyName;
        private string _userName;

        public XpandAuditDataItemPersistent(Session session, string userName, DateTime modifiedOn, string description)
            : base(session){
            _userName = userName;
            _modifiedOn = modifiedOn;
            Description = description;
        }

        public XpandAuditDataItemPersistent(Session session)
            : base(session) {
        }

        [Indexed]
        public string UserName{
            get { return _userName; }
            set { SetPropertyValue("UserName", ref _userName, value); }
        }

        [PersistentAlias("AuditedObject.TargetTypeName")]
        public string TargetTypeName{
            get { return EvaluateAlias("TargetTypeName") as string; }
        }

        [PersistentAlias("AuditedObject.DisplayName")]
        public string DisplayName{
            get { return EvaluateAlias("DisplayName") as string; }
        }

        [Indexed]
        public DateTime ModifiedOn{
            get { return _modifiedOn; }
            set { SetPropertyValue("ModifiedOn", ref _modifiedOn, value); }
        }

        [Indexed]
        public string OperationType{
            get { return _operationType; }
            set { SetPropertyValue("OperationType", ref _operationType, value); }
        }

        [Size(SizeAttribute.Unlimited), Delayed, MemberDesignTimeVisibility(true)]
        public string Description{
            get { return GetDelayedPropertyValue<string>("Description"); }
            set { SetDelayedPropertyValue("Description", value); }
        }

        public override void AfterConstruction(){
            base.AfterConstruction();
            _pending = true;
        }

        [InvisibleInAllViews]
        public bool Pending{
            get { return _pending; }
            set { SetPropertyValue("Pending", ref _pending, value); }
        }

        [Association("XpandAuditedObjectWeakReference-XpandAuditDataItemPersistent"), MemberDesignTimeVisibility(false)]
        public XpandAuditedObjectWeakReference AuditedObject{
            get { return _auditedObject; }
            set { SetPropertyValue("AuditedObject", ref _auditedObject, value); }
        }

        [Aggregated, MemberDesignTimeVisibility(false)]
        public XPWeakReference OldObject{
            get { return _oldObject; }
            set { SetPropertyValue("OldObject", ref _oldObject, value); }
        }

        [Aggregated, MemberDesignTimeVisibility(false)]
        public XPWeakReference NewObject{
            get { return _newObject; }
            set { SetPropertyValue("NewObject", ref _newObject, value); }
        }

        [Delayed, Size(SizeAttribute.Unlimited)]
        public string OldValue{
            get { return GetDelayedPropertyValue<string>("OldValue"); }
            set { SetDelayedPropertyValue("OldValue", value); }
        }

        [Delayed, Size(SizeAttribute.Unlimited)]
        public string NewValue{
            get { return GetDelayedPropertyValue<string>("NewValue"); }
            set { SetDelayedPropertyValue("NewValue", value); }
        }

        public string PropertyName{
            get { return _propertyName; }
            set { SetPropertyValue("PropertyName", ref _propertyName, value); }
        }
    }
}