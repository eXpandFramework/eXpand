
namespace Xpand.ExpressApp.Web.Core
{
    public class DynamicDouplicateTypesMapper : Xpand.ExpressApp.Core.DynamicModel.DynamicDouplicateTypesMapper
    {
        public DynamicDouplicateTypesMapper() {
            Add(typeof(DevExpress.Utils.DefaultBoolean),typeof(eXpandDefaultBoolean));
            Add(typeof(DevExpress.Web.ColumnFilterMode), typeof(eXpandColumnFilterMode));
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
