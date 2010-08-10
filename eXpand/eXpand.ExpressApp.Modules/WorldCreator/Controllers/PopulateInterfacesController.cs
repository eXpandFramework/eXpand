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
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.WorldCreator.Controllers
{
    [ModelNodesGenerator(typeof(ModelInterfaceSourcesNodesGenerator))]
    public interface IModelInterfaceSources : IModelNode, IModelList<IModelAssemblyResourceImageSource>
    {
    }

    public class ModelInterfaceSourcesNodesGenerator:ModelNodesGeneratorBase {
        protected override void GenerateNodesCore(ModelNode node) {
            
        }
    }

    public class PopulateInterfacesController : ViewController<DevExpress.ExpressApp.ListView>, IModelExtender
    {
        public PopulateInterfacesController()
        {
            TargetObjectType = typeof(IInterfaceInfo);
            var populateInterfaces = new SimpleAction(Container)
            {
                Caption = "Populate",
                Category = PredefinedCategory.Search.ToString(),
                Id = "populateInterfaces"
            };
            populateInterfaces.Execute += PopulateInterfacesOnExecute;
            Actions.Add(populateInterfaces);
        }

        void PopulateInterfacesOnExecute(object sender, SimpleActionExecuteEventArgs args)
        {
            createInterfaces(View.CollectionSource);
        }


        void createInterfaces(CollectionSourceBase collectionSourceBase)
        {
            var iface = ((IInterfaceInfo)ObjectSpace.CreateObject(View.ObjectTypeInfo.Type));
            ObjectSpace.Session.Delete(iface);
            string assemblyName = iface.GetPropertyInfo(x => x.Assembly).Name;
            string name = iface.GetPropertyInfo(x => x.Name).Name;
            foreach (Type type in getInterfaces()){
                if (ObjectSpace.Session.FindObject(View.ObjectTypeInfo.Type,CriteriaOperator.Parse(string.Format("{0}=? AND {1}=?", assemblyName, name),
                    new AssemblyName(type.Assembly.FullName + "").Name, type.FullName)) == null){
                    createInterfaceInfo(type, collectionSourceBase);
                }
            }
            ObjectSpace.CommitChanges();
        }

        void createInterfaceInfo(Type type, CollectionSourceBase collectionSourceBase)
        {
            var info = (IInterfaceInfo)ObjectSpace.CreateObject(View.ObjectTypeInfo.Type);
            info.Name = type.FullName;
            info.Assembly = new AssemblyName(type.Assembly.FullName + "").Name;
            collectionSourceBase.Add(info);
        }


        IEnumerable<Type> getInterfaces()
        {
            IEnumerable<string> assemblyNames = ((IModelInterfaceSources)Application.Model).Select(
                    node => node.AssemblyName);
            IEnumerable<Assembly> assemblies =
                AppDomain.CurrentDomain.GetAssemblies().Where(
                    assembly => assemblyNames.Contains(new AssemblyName(assembly.FullName + "").Name));
            var types = new List<Type>();
            foreach (Assembly assembly in assemblies){
                types.AddRange(assembly.GetTypes().Where(type => type.IsInterface));
            }
            return types;
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelApplication, IModelInterfaceSources>();
        }
    }
}