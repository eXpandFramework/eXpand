using System.Reflection;

namespace eXpand.ExpressApp.Core.DynamicModel {
    public interface IDynamicModelPropertiesSource
    {
        PropertyInfo[] GetProperties();
    }
}