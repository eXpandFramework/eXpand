using System.Linq;
using DevExpress.ExpressApp;
using TypesInfo = eXpand.ExpressApp.ImportExport.Core.TypesInfo;

namespace eXpand.ExpressApp.ImportExport {
    public sealed partial class ImportExportModule : ModuleBase
    {
        
        public ImportExportModule(){
            InitializeComponent();
        }
        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            TypesInfo.Instance.AddTypes(Application.Modules.SelectMany(@base => @base.AdditionalBusinessClasses));
        }

    }
}