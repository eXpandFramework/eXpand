using System.Reflection;

namespace Xpand.ExpressApp.Core.DynamicModel {
    public interface IDynamicModelPropertiesSource
    {
        PropertyInfo[] GetProperties();
    }
}