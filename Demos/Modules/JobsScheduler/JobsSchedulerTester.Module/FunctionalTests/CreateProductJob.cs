using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Quartz;
using Xpand.ExpressApp.JobScheduler.QuartzExtensions;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Persistent.BaseImpl.JobScheduler;

namespace JobsSchedulerTester.Module.FunctionalTests{
    [JobDetailDataMapType(typeof(ProductDetailDataMap))]
    [System.ComponentModel.DisplayName("ProductJob")]
    public class CreateProductJob:IJob{
        public void Execute(IJobExecutionContext context){
            var application = ((IXpandScheduler)context.Scheduler).Application;
            
            var objectSpaceProvider = application.ObjectSpaceProvider;
            using (var objectSpace = objectSpaceProvider.CreateObjectSpace()){
                var product = objectSpace.CreateObject<Product>();
                var jobDataMap = context.JobDetail.JobDataMap;
                product.Parts = (int) jobDataMap.Get<ProductDetailDataMap>(map => map.Parts);
                objectSpace.CommitChanges();
            }
        }

    }

    [CreatableItem]
    public class ProductDetailDataMap : XpandJobDetailDataMap {
        public ProductDetailDataMap(Session session) : base(session){
        }

        public int Parts { get; set; }
    }
}