using System;
using System.Reflection;

namespace Xpand.Utils.DependentAssembly {
    public interface IDependentAssemblyAttribute {
        Assembly Assembly { get; }
    }

    [Obsolete("You not need it", true)]
    public class DependentAssemblyAttribute : Attribute, IDependentAssemblyAttribute {
        readonly Type _type;

        public DependentAssemblyAttribute(Type type)
        {
            _type = type;
        }

        public Assembly Assembly
        {
            get { return _type.Assembly; }
        }
    }
}