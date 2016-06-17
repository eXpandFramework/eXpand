using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;

namespace Xpand.ExpressApp.WorldCreator.System.NodeUpdaters {
    public class ImageSourcesUpdater : ModelNodesGeneratorUpdater<ImageSourceNodesGenerator> {
        readonly ModuleBase[] _definedModules;

        public ImageSourcesUpdater(ModuleBase[] definedModules) {
            _definedModules = definedModules;
        }

        public override void UpdateNode(ModelNode node) {
            var modelImageSources = (IModelImageSources)node;
            var modelAssemblyResourceImageSources =
                _definedModules.Select(definedModule => new AssemblyName(definedModule.GetType().Assembly.FullName + "").Name).
                    Select(name => modelImageSources.OfType<IModelAssemblyResourceImageSource>().First(s => s.AssemblyName == name));
            foreach (var source in modelAssemblyResourceImageSources) {
                source.Remove();
                source.Remove();
            }
        }
    }
}