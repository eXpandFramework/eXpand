using System.Linq;
using DevExpress.Persistent.AuditTrail;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.AuditTrail {
    public class NoAuditProccesor : ObjectAuditProcessor {
        public NoAuditProccesor(Session session, AuditTrailSettings settings)
            : base(session, settings) {
        }

        public override bool IsObjectAudited(object obj) {
            return false;
        }
    }

    public class XpandObjectAuditProcessorsFactory : ObjectAuditProcessorsFactory {
        public override bool IsSuitableAuditProcessor(ObjectAuditProcessor processor, ObjectAuditingMode mode) {
            var isNoAuditMode = mode == (ObjectAuditingMode)Logic.ObjectAuditingMode.None;
            if (isNoAuditMode) {
                return processor is NoAuditProccesor;
            }
            return base.IsSuitableAuditProcessor(processor, mode);
        }

        public override ObjectAuditProcessor CreateAuditProcessor(ObjectAuditingMode mode, Session session, AuditTrailSettings settings) {
            var auditTrailSettings = new AuditTrailSettings();
            auditTrailSettings.SetXPDictionary(XpandModuleBase.Dictiorary);
            foreach (var auditTrailClassInfo in settings.TypesToAudit) {
                var auditTrailMemberInfos = auditTrailClassInfo.Properties;
                auditTrailSettings.AddType(auditTrailClassInfo.ClassInfo.ClassType, auditTrailMemberInfos.Select(info => info.Name).ToArray());
            }
            return mode == (ObjectAuditingMode)Logic.ObjectAuditingMode.None ? new NoAuditProccesor(session, auditTrailSettings) : base.CreateAuditProcessor(mode, session, auditTrailSettings);
        }
    }

}
