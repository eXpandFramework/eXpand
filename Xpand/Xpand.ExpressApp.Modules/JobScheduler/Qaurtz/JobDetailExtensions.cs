using System;
using System.Linq;
using DevExpress.ExpressApp.DC;
using Quartz;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.JobScheduler.Qaurtz {
    public static class JobDetailExtensions {
        public static void AssignXpandJobDetail(this JobDetail jobDetail, IJobDetail xpandJobDetail) {
            jobDetail.Name = xpandJobDetail.Name;
            jobDetail.Description = xpandJobDetail.Description;
            jobDetail.Group = xpandJobDetail.Job.JobType.FullName;
            jobDetail.JobType = xpandJobDetail.Job.JobType;
            jobDetail.RequestsRecovery = xpandJobDetail.RequestsRecovery;
        }
        public static JobDetail CreateQuartzJobDetail(this IJobDetail xpandJobDetail) {
            var jobDetail = new JobDetail();
            jobDetail.AssignXpandJobDetail(xpandJobDetail);
            return jobDetail;
        }
        static Func<IMemberInfo, int, bool> CanBeMapped() {
            return (info, i) => info.IsPersistent && info.IsPublic && !info.IsReadOnly && info.MemberType.IsSerializable && info.FindAttribute<NonDataMapMember>() == null;
        }

        public static void AssignDataMap(this JobDetail jobDetail, ITypeInfo typeInfo, IDataMap jobDataMap) {
            typeInfo.Members.Where(CanBeMapped()).Each(info => jobDetail.JobDataMap.MapValue(jobDataMap, info));
        }

    }
}