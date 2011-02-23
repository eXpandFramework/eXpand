using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using Quartz;

namespace Xpand.Persistent.BaseImpl.JobScheduler {
    public class JobTypeClassInfoConverter : ClassInfoTypeConverter {
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
            var typeInfo = XafTypesInfo.Instance.FindTypeInfo(typeof(IJob));
            var typeInfos = ReflectionHelper.FindTypeDescendants(typeInfo).Select(info => info.Type).ToList();
            return new StandardValuesCollection(typeInfos);
        }
    }
}