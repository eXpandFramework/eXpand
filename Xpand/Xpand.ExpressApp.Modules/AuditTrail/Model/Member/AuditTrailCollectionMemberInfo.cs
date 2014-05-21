using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.Persistent.Base.General;
using Xpand.Xpo.MetaData;

namespace Xpand.ExpressApp.AuditTrail.Model.Member {
    public class AuditTrailCollectionMemberInfo : XpandCustomMemberInfo {
        

        public AuditTrailCollectionMemberInfo(XPClassInfo owner, string propertyName, Type propertyType)
            : base(owner, propertyName, typeof(XPCollection<>).MakeGenericType(propertyType), null, true, false) {
        }


        public override object GetValue(object theObject) {
            var xpBaseObject = ((XPBaseObject)theObject);
            return base.GetStore(theObject).GetCustomPropertyValue(this) == null
                       ? CreateInstance(xpBaseObject)
                       : base.GetValue(theObject);
        }

        object CreateInstance(XPBaseObject xpBaseObject) {
            var auditedObjectWeakReferenceType = ApplicationHelper.Instance.Application.TypesInfo.FindTypeInfo("DevExpress.Persistent.BaseImpl.AuditedObjectWeakReference").Type;
            return GetAuditTrail(xpBaseObject.Session, xpBaseObject, auditedObjectWeakReferenceType);
        }

        XPBaseCollection GetAuditTrail(Session session, XPBaseObject xpBaseObject, Type auditedObjectWeakReferenceType) {
            var binaryOperator = new BinaryOperator("TargetType", session.GetObjectType(xpBaseObject));
            var operands = new BinaryOperator("TargetKey", XPWeakReference.KeyToString(session.GetKeyValue(xpBaseObject)));
            var auditObjectWR = (XPWeakReference) session.FindObject(auditedObjectWeakReferenceType,
                                                                     new GroupOperator(binaryOperator,operands));
            if (auditObjectWR != null) {
                var baseCollection = (XPBaseCollection) auditObjectWR.ClassInfo.GetMember("AuditDataItems").GetValue(auditObjectWR);
                baseCollection.BindingBehavior = CollectionBindingBehavior.AllowNone;
                return baseCollection;
            }
            return null;
        }


        protected override bool CanPersist {
            get { return false; }
        }
    }
}