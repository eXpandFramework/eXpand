using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace Xpand.ExpressApp.ImportWizard.Win.Wizard
{
    public delegate void ExcelImportWizard_StringToPropertyMap(XPObjectSpace objectSpace, XPMemberInfo prop, string value, ref IXPSimpleObject newObj);
}