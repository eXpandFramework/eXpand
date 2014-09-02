using System.Collections.Generic;
using System.Reflection;

namespace Xpand.Persistent.Base.ModelDifference{
    public class ResourceInfo{
        private readonly List<AspectInfo> _aspectInfos = new List<AspectInfo>();

        public ResourceInfo(string name, Assembly assembly){
            Name = name;
            AssemblyName = new AssemblyName(assembly.FullName).Name;
        }

        public string Name { get; set; }
        public string AssemblyName { get; set; }

        public List<AspectInfo> AspectInfos{
            get { return _aspectInfos; }
        }
    }
}