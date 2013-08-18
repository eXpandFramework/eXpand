using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.AuditTrail;
using DevExpress.Persistent.Base;
using System.Linq;
using Xpand.ExpressApp.AuditTrail.Logic;

namespace Xpand.ExpressApp.AuditTrail.Model {
//    public interface IAudiTrailInfo:IModelNode {
//        ObjectAuditingMode? AuditingMode { get; set; }
//    }

    public interface IAuditTrailSettingsMembers : IModelNode, IModelList<IAuditTrailSettingsMember> {
         
    }
    [KeyProperty("Name")]
    public interface IAuditTrailSettingsMember:IModelNode {
        [Required]
        [DataSourceProperty("Names")]
        string Name { get; set; }
        [Browsable(false)]
        IEnumerable<string> Names { get; }
    }
    [DomainLogic(typeof(IAuditTrailSettingsMember))]
    public class AuditTrailSettingsMembersDomainLogic {
//        public static IEnumerable<string> Get_Names(IAuditTrailSettingsMember auditTrailSettingsMember) {
//            return ((IAuditTrailRule)auditTrailSettingsMember.Parent.Parent).ModelClass.AllMembers.Select(member => member.Name);
//        }
    }
}
