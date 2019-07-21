//using System.Collections.Generic;
//using DevExpress.DashboardWin;
//using DevExpress.ExpressApp;
//using DevExpress.ExpressApp.Model;
//using Xpand.ExpressApp.Dashboard;
//using Xpand.Persistent.Base.General;
//using Xpand.Persistent.Base.ModelAdapter;
//
//namespace Xpand.ExpressApp.XtraDashboard.Win.Controllers {
//    public interface IModelDashboardModuleDesignerSettings:IModelNode{
//        IModelDashboardDesignerSettings DesignerSettings { get; }
//    }
//    public interface IModelDashboardDesignerSettings : IModelNode {
//    }
//
//    public class DashboardDesignerModelAdapterController:ModelAdapterController,IModelExtender {
//        protected override void OnActivated(){
//            base.OnActivated();
//            Frame.GetController<DashboardDesignerController>(controller => controller.DashboardDesignerOpening += OnDashboardDesignerOpening);
//        }
//
//        protected override void OnDeactivated(){
//            base.OnDeactivated();
//            Frame.GetController<DashboardDesignerController>(controller => controller.DashboardDesignerOpening -= OnDashboardDesignerOpening);
//        }
//
//        private void OnDashboardDesignerOpening(object sender, Dashboard.Controllers.DashboardDesignerOpeningEventArgs e){
//            var args = ((DashboardDesignerOpeningEventArgs) e);
//            var settings = ((IModelDashboardModuleDesignerSettings) ((IModelApplicationDashboardModule) Application.Model).DashboardModule).DesignerSettings;
//            new ObjectModelSynchronizer(args.Designer.DataSourceWizardSettings,settings).ApplyModel();
//        }
//
//        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
//            extenders.Add<IModelDashboardModule, IModelDashboardModuleDesignerSettings>();
//            var builder = new InterfaceBuilder(extenders);
//            var assembly = builder.Build(BuilderDatas(), GetPath(typeof(DashboardDataSourceWizardSettings).Name));
//            builder.ExtendInteface<IModelDashboardDesignerSettings, DashboardDataSourceWizardSettings>(assembly);
//        }
//
//        private static IEnumerable<InterfaceBuilderData> BuilderDatas() {
//            yield return new InterfaceBuilderData(typeof(DashboardDataSourceWizardSettings)) {
//                Act = info => (info.DXFilter())
//            };
//        }
//    }
//}
