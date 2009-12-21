using System.Linq;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.IO.Core;

namespace eXpand.ExpressApp.IO {
    public sealed partial class IOModule : ModuleBase
    {
        
        public IOModule(){
            InitializeComponent();
        }
        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            TypesInfo.Instance.AddTypes(Application.Modules.SelectMany(@base => @base.AdditionalBusinessClasses));
        }

    }
}