using System;
using DevExpress.Persistent.AuditTrail;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General.CustomAttributes;
using Xpand.XAF.Modules.CloneModelView;
using Xpand.Xpo;

namespace Xpand.ExpressApp.AuditTrail.BusinessObjects {
    [ImageName("BO_Audit_ChangeHistory")]
    [CloneModelView(CloneViewType.ListView, "AuditDataItemPersistent_Pending_ListView")]
    [CloneModelView(CloneViewType.ListView, "AuditDataItemPersistent_Approved_ListView")]
    [System.ComponentModel.DisplayName("AuditDataItemPersistent")]
    public class XpandAuditDataItemPersistent : XpandCustomObject,
        IAuditDataItemPersistent<XpandAuditedObjectWeakReference> {
        private XpandAuditedObjectWeakReference _auditedObject;
        private DateTime _modifiedOn;
        private XPWeakReference _newObject;
        private XPWeakReference _oldObject;
        private string _operationType;
        private bool _pending;
        private string _propertyName;
        private string _userName;

        public XpandAuditDataItemPersistent(Session session, string userName, DateTime modifiedOn, string description)
            : base(session) {
            _userName = userName;
            _modifiedOn = modifiedOn;
            Description = description;
        }

        public XpandAuditDataItemPersistent(Session session)
            : base(session) {
        }

        [PersistentAlias("AuditedObject.TargetTypeName")]
        public string TargetTypeName => EvaluateAlias("TargetTypeName") as string;

        [PersistentAlias("AuditedObject.DisplayName")]
        public string DisplayName => EvaluateAlias("DisplayName") as string;

        [InvisibleInAllViews]
        public bool Pending {
            get => _pending;
            set => SetPropertyValue("Pending", ref _pending, value);
        }

        [Indexed]
        public string UserName {
            get => _userName;
            set => SetPropertyValue("UserName", ref _userName, value);
        }

        [Indexed]
        public DateTime ModifiedOn {
            get => _modifiedOn;
            set => SetPropertyValue("ModifiedOn", ref _modifiedOn, value);
        }

        [Indexed]
        public string OperationType {
            get => _operationType;
            set => SetPropertyValue("OperationType", ref _operationType, value);
        }

        [Size(SizeAttribute.Unlimited)]
        [Delayed]
        [MemberDesignTimeVisibility(true)]
        public string Description {
            get => GetDelayedPropertyValue<string>("Description");
            set => SetDelayedPropertyValue("Description", value);
        }

        [Association("XpandAuditedObjectWeakReference-XpandAuditDataItemPersistent")]
        [MemberDesignTimeVisibility(false)]
        public XpandAuditedObjectWeakReference AuditedObject {
            get => _auditedObject;
            set => SetPropertyValue("AuditedObject", ref _auditedObject, value);
        }

        [Aggregated]
        [MemberDesignTimeVisibility(false)]
        public XPWeakReference OldObject {
            get => _oldObject;
            set => SetPropertyValue("OldObject", ref _oldObject, value);
        }

        [Aggregated]
        [MemberDesignTimeVisibility(false)]
        public XPWeakReference NewObject {
            get => _newObject;
            set => SetPropertyValue("NewObject", ref _newObject, value);
        }

        [Delayed]
        [Size(SizeAttribute.Unlimited)]
        public string OldValue {
            get => GetDelayedPropertyValue<string>("OldValue");
            set => SetDelayedPropertyValue("OldValue", value);
        }

        [Delayed]
        [Size(SizeAttribute.Unlimited)]
        public string NewValue {
            get => GetDelayedPropertyValue<string>("NewValue");
            set => SetDelayedPropertyValue("NewValue", value);
        }

        public string PropertyName {
            get => _propertyName;
            set => SetPropertyValue("PropertyName", ref _propertyName, value);
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            _pending = true;
        }
    }
}