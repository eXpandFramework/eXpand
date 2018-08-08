using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.WorldCreator.Controllers {
    [ModelNodesGenerator(typeof(ModelInterfaceSourcesNodesGenerator))]
    public interface IModelInterfaceSources : IModelNode, IModelList<IModelInterfaceSource> {
    }
    [KeyProperty("AssemblyName")]
    public interface IModelInterfaceSource:IModelNode {
        [Required]
        string AssemblyName { get; set; }
    }

    public interface IModelApplicationInterfaceInfoSource {
        IModelInterfaceSources InterfaceSources { get; }
    }

    public class ModelInterfaceSourcesNodesGenerator : ModelNodesGeneratorBase {
        protected override void GenerateNodesCore(ModelNode node) {

        }
    }

    public class PopulateInterfacesController : ViewController<ListView>, IModelExtender {
        public PopulateInterfacesController() {
            TargetObjectType = typeof(IInterfaceInfo);
            var populateInterfaces = new SimpleAction(Container) {
                Caption = "Populate",
                Category = PredefinedCategory.Search.ToString(),
                Id = "populateInterfaces"
            };
            populateInterfaces.Execute += PopulateInterfacesOnExecute;
            Actions.Add(populateInterfaces);
        }

        void PopulateInterfacesOnExecute(object sender, SimpleActionExecuteEventArgs args) {
            CreateInterfaces(View.CollectionSource);
        }

        void CreateInterfaces(CollectionSourceBase collectionSourceBase) {
            var interfaceInfo = ((IInterfaceInfo)ObjectSpace.CreateObject(View.ObjectTypeInfo.Type));
            ObjectSpace.Delete(interfaceInfo);
            string assemblyName = interfaceInfo.GetPropertyInfo(x => x.Assembly).Name;
            string name = interfaceInfo.GetPropertyInfo(x => x.Name).Name;
            foreach (Type type in GetInterfaces()) {
                if (ObjectSpace.FindObject(View.ObjectTypeInfo.Type, CriteriaOperator.Parse(
                        $"{assemblyName}=? AND {name}=?",
                    new AssemblyName(type.Assembly.FullName + "").Name, type.FullName)) == null) {
                    CreateInterfaceInfo(type, collectionSourceBase);
                }
            }
            ObjectSpace.CommitChanges();
        }

        void CreateInterfaceInfo(Type type, CollectionSourceBase collectionSourceBase) {
            var info = (IInterfaceInfo)ObjectSpace.CreateObject(View.ObjectTypeInfo.Type);
            info.Name = type.FullName;
            info.Assembly = new AssemblyName(type.Assembly.FullName + "").Name;
            collectionSourceBase.Add(info);
        }

        IEnumerable<Type> GetInterfaces() {
            var assemblyNames = ((IModelApplicationInterfaceInfoSource)Application.Model).InterfaceSources.Select(node => node.AssemblyName);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToAssemblyDefinition();
            return assemblies.Where(assembly=> assemblyNames.Contains(new AssemblyName(assembly.FullName + "").Name)).SelectMany(assembly 
                    => assembly.MainModule.Types).Where(type => type.IsInterface).Select(definition => AppDomain.CurrentDomain.FindType(definition));
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelApplication, IModelApplicationInterfaceInfoSource>();
        }
    }
}