using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using System.Linq;

namespace Xpand.Persistent.Base.General.Model {

    [ModelNodesGenerator(typeof (ModuleNodesGenerator))]
    public interface IModelModules : IModelNode, IModelList<IModelModule> {
    }

    public interface IModelApplicationModule {
        [ModelPersistentName("Modules")]
        IModelModules ModulesList { get;  }
    }
    [KeyProperty("Name"), DisplayProperty("Name")]
    public interface IModelModule:IModelNode {
        string Name { get; set; }
    }

    public class ModuleNodesGenerator : ModelNodesGeneratorBase {
        protected override void GenerateNodesCore(ModelNode node) {
            foreach (var module in ((IModelSources) node.Application).Modules) {
                var modelModule = node.AddNode<IModelModule>();
                modelModule.Name = module.GetType().FullName;
            }
//            var findTypeInfo = XafTypesInfo.Instance.FindTypeInfo(typeof(ModuleBase));
//            var findTypeDescendants = ReflectionHelper.FindTypeDescendants(findTypeInfo).Where(ModulesFilterPredicate(node));
//            foreach (ITypeInfo typeInfo in findTypeDescendants){
//                var module = node.AddNode<IModelModule>();
//                module.Name = typeInfo.FullName;
//            }
        }

        Func<ITypeInfo, bool> ModulesFilterPredicate(ModelNode node) {
            return info => !info.Type.IsAbstract && !info.Type.IsGenericType && node[info.FullName]==null;
        }
    }
}