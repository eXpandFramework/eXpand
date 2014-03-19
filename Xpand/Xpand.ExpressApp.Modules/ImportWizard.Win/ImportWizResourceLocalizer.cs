using System.ComponentModel;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Win.Templates;
using Xpand.ExpressApp.ImportWizard.Win.Wizard;

namespace Xpand.ExpressApp.ImportWizard.Win{
    public class ImportWizResourceLocalizer : XafResourceLocalizer{
        [Localizable(false)]
        protected override IXafResourceManagerParameters GetXafResourceManagerParameters(){
            return new XafResourceManagerParameters(new[]{"Captions", "ImportWiz"},
                "Xpand.ExpressApp.ImportWizard.Win.Properties.Resources", "ExcelImportWizard_", GetType().Assembly);
//                "Xpand.ExpressApp.ImportWizard.Win.Wizard.ExcelImportWizard", "", GetType().Assembly);
        }
    }

    [Localizable(false)]
    public class ImportWizFrameTemplateLocalizer : FrameTemplateLocalizer<ExcelImportWizard>{
        protected override IXafResourceManagerParameters GetXafResourceManagerParameters(){
            return new XafResourceManagerParameters(new[]{"Captions", "ImportWiz"},
                "Xpand.ExpressApp.ImportWizard.Win.Wizard.ExcelImportWizard", "", GetType().Assembly);
        }
    }
}