using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Xpo;

namespace SecurityDemo.Win{
    public partial class SecurityDemoWindowsFormsApplication : WinApplication{
        public SecurityDemoWindowsFormsApplication(){
            InitializeComponent();
        }



        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args){
            args.ObjectSpaceProvider = new XPObjectSpaceProvider(args.ConnectionString, args.Connection,true);
        }
    }
}