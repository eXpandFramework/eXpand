using ColumnFilterMode = DevExpress.Web.ASPxGridView.ColumnFilterMode;
using DefaultBoolean = DevExpress.Web.ASPxClasses.DefaultBoolean;

namespace Xpand.ExpressApp.Web.Core
{
    public class DynamicDouplicateTypesMapper : Xpand.ExpressApp.Core.DynamicModel.DynamicDouplicateTypesMapper
    {
        public DynamicDouplicateTypesMapper() {
            Add(typeof(DefaultBoolean),typeof(eXpandDefaultBoolean));
            Add(typeof(ColumnFilterMode), typeof(eXpandColumnFilterMode));
        }
    }

    public enum eXpandColumnFilterMode
    {
        Value, DisplayText
    }
    public enum eXpandDefaultBoolean
    {
        True,
        False,
        Default
    }
}
