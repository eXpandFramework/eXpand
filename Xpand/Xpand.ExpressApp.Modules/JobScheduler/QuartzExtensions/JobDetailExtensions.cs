using System;
using System.Linq;
using DevExpress.ExpressApp.DC;
using Quartz.Impl;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.JobScheduler.QuartzExtensions {
    public static class JobDetailExtensions {
        public static void AssignXpandJobDetail(this JobDetailImpl jobDetail, IXpandJobDetail xpandJobDetail) {
            jobDetail.Name = xpandJobDetail.Name;
            jobDetail.Description = xpandJobDetail.Description;
            jobDetail.Group = xpandJobDetail.Job.JobType.FullName;
            jobDetail.JobType = xpandJobDetail.Job.JobType;
            jobDetail.RequestsRecovery = xpandJobDetail.RequestsRecovery;
        }
        public static JobDetailImpl CreateQuartzJobDetail(this IXpandJobDetail xpandJobDetail) {
            var jobDetail = new JobDetailImpl();
            jobDetail.AssignXpandJobDetail(xpandJobDetail);
            return jobDetail;
        }
        static Func<IMemberInfo, int, bool> CanBeMapped() {
            return (info, i) => (info.IsPersistent && info.IsPublic && !info.IsReadOnly && info.MemberType.IsSerializable && info.FindAttribute<NonDataMapMember>() == null) || (info.MemberTypeInfo.IsPersistent);
        }

        public static void AssignDataMap(this JobDetailImpl jobDetail, ITypeInfo typeInfo, IDataMap jobDataMap) {
            typeInfo.Members.Where(CanBeMapped()).Each(info => jobDetail.JobDataMap.MapValue(jobDataMap, info));
        }

    }
}