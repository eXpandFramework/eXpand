using System.Collections.Generic;

namespace Xpand.ExpressApp.ModelEditor.ModelDifference {
    public class ResourceInfo {
        public ResourceInfo(string name, string assemblyName) {
            Name = name;
            AssemblyName = assemblyName;
        }

        public string Name { get; set; }
        public string AssemblyName { get; set; }
        private readonly List<AspectInfo> _aspectInfos = new List<AspectInfo>();
        public List<AspectInfo> AspectInfos {
            get { return _aspectInfos; }
        }

    }
}