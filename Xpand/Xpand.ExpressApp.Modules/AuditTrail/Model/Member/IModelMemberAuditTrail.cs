
using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.AuditTrail;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.AuditTrail;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.RuntimeMembers.Model;
using Xpand.Persistent.Base.RuntimeMembers.Model.Collections;

namespace Xpand.ExpressApp.AuditTrail.Model.Member {
    [ModelDisplayName("AuditTrail")]
    public interface IModelMemberAuditTrail:IModelMemberColection {
         
    }

    [DomainLogic(typeof(IModelMemberAuditTrail))]
    public class ModelMemberAuditTrailDomainLogic : ModelMemberExDomainLogicBase<IModelMemberAuditTrail> {
        public static IModelList<IModelClass> Get_CollectionTypes(IModelMemberAuditTrail orphanedColection) {
            return new CalculatedModelNodeList<IModelClass>(orphanedColection.Application.BOModel.Where(@class => typeof(IBaseAuditDataItemPersistent).IsAssignableFrom(@class.TypeInfo.Type)));
        }

        public static IModelClass Get_CollectionType(IModelMemberAuditTrail auditTrail) {
            XafApplication application = ApplicationHelper.Instance.Application;
            if (application != null) {
                var auditTrailModule = application.Modules.FindModule<AuditTrailModule>();
                Type collectionType = auditTrailModule.AuditDataItemPersistentType;
                return auditTrail.Application.BOModel.GetClass(collectionType);
            }
            return auditTrail.CollectionTypes.Count == 1 ? auditTrail.CollectionTypes.FirstOrDefault() : null;
        }
        public static IMemberInfo Get_MemberInfo(IModelMemberAuditTrail memberAuditTrail) {
            return GetMemberInfo(memberAuditTrail,
                (colection, info) => new AuditTrailCollectionMemberInfo(info, colection.Name, colection.CollectionType.TypeInfo.Type),
                colection => colection.CollectionType != null);
        }

    }

}