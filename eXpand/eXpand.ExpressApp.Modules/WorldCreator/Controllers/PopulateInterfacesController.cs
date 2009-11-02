using System;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.ExpressApp;
using eXpand.Persistent.Base.PersistentMetaData;
using System.Linq;

namespace eXpand.ExpressApp.WorldCreator.Controllers
{
    public partial class PopulateInterfacesController : ViewController<ListView>
    {
        public const string InterfaceSourcesAttributeName = "InterfaceSources";
        public PopulateInterfacesController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof (IInterfaceInfo);
            TargetViewType=ViewType.ListView;
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            if (!(Frame is NestedFrame)) {
                createInterfaces(View.CollectionSource);
            }
        }

        private void createInterfaces(CollectionSourceBase collectionSourceBase) {
            
            ObjectSpace.Session.Delete(collectionSourceBase.Collection);
                foreach (Type type in getInterfaces()){
                    var iface = ((IInterfaceInfo)ObjectSpace.CreateObject(View.ObjectTypeInfo.Type));
                    iface.Name = type.FullName;
                    var assemblyName = type.Assembly.FullName;
                    iface.Assembly = new AssemblyName(assemblyName+"").Name;
                    collectionSourceBase.Add(iface);   
                }
        }


        private IEnumerable<Type> getInterfaces() {
            var assemblyNames = Application.Model.RootNode.GetChildNode(InterfaceSourcesAttributeName).ChildNodes.Select(node => node.KeyAttribute.Value);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assemblyNames.Contains(new AssemblyName(assembly.FullName+"").Name));
            var types = new List<Type>();
            foreach (var assembly in assemblies) {
                types.AddRange(assembly.GetTypes().Where(type => type.IsInterface));
            }
            return types;
        }
        public override void UpdateModel(Dictionary dictionary)
        {
            base.UpdateModel(dictionary);
            dictionary.RootNode.GetChildNode(InterfaceSourcesAttributeName);
        }
        public override Schema GetSchema()
        {
            return new Schema(new DictionaryXmlReader().ReadFromString(
                @"<Element Name=""Application"">
					<Element Name=""" +InterfaceSourcesAttributeName+ @""">
						<Element Name=""AssemblyResourceImageSource"" KeyAttribute=""AssemblyName"" Multiple=""True"">
			                <Attribute Name=""AssemblyName"" Required=""True""/>
					    </Element>
                    </Element>
				</Element>"));
        }
    }

}
