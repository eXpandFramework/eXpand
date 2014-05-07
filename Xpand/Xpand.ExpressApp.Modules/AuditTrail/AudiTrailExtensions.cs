using System.Collections.Generic;
using DevExpress.Persistent.AuditTrail;
using DevExpress.Xpo;
using Fasterflect;

namespace Xpand.ExpressApp.AuditTrail {
    public static class AudiTrailExtensions {
        public static ObjectAuditProcessor GetSessionAuditProcessor(this AuditTrailService auditTrailService, Session session) {
            var objectAuditProcessors = ((Dictionary<Session, ObjectAuditProcessor>) auditTrailService.GetFieldValue("objectAuditProcessors"));
            return objectAuditProcessors.ContainsKey(session) ? objectAuditProcessors[session] : null;
        }
    }
}
