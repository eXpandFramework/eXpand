using DevExpress.ExpressApp;

namespace JobsSchedulerTester.Module {
    public static class Extensions {
        public static void ProjectSetup(this XafApplication application) {
            application.OptimizedControllersCreation = true;
            
        }
    }
}
