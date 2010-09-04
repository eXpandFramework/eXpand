using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;

namespace Xpand.ExpressApp.WorldCreator.NodeUpdaters {
    public class ImageSourcesUpdater : ModelNodesGeneratorUpdater<ImageSourceNodesGenerator>
    {
        readonly List<Type> _definedModules;

        public ImageSourcesUpdater(List<Type> definedModules) {
            _definedModules = definedModules;
        }

        public override void UpdateNode(ModelNode node) {
            var modelImageSources = (IModelImageSources) node;
            var modelAssemblyResourceImageSources =
                _definedModules.Select(definedModule => new AssemblyName(definedModule.Assembly.FullName + "").Name).
                    Select(name => modelImageSources.OfType<IModelAssemblyResourceImageSource>().First(s => s.AssemblyName == name));
            foreach (var source in modelAssemblyResourceImageSources)
            {
                modelImageSources.Remove(source);
            }
        }
    }
}