using System;
using System.Reflection;

namespace eXpand.Utils.DependentAssembly {
    public class DependentAssemblyAttribute : Attribute
    {
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