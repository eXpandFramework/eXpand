using System;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using eXpand.Persistent.Base.PersistentMetaData;
using System.Linq;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.WorldCreator.Controllers
{
    public partial class PopulateInterfacesController : ViewController<ListView>
    {
        public const string InterfaceSourcesAttributeName = "InterfaceSources";
        public PopulateInterfacesController()
        {
            InitializeComponent();
            
            TargetObjectType = typeof (IInterfaceInfo);
            var populateInterfaces = new SimpleAction(components) {Caption = "Populate",Category = PredefinedCategory.Search.ToString(),Id = "populateInterfaces"};
            populateInterfaces.Execute+=PopulateInterfacesOnExecute;
            RegisterActions(components);
            TargetViewType=ViewType.ListView;
            
        }

        private void PopulateInterfacesOnExecute(object sender, SimpleActionExecuteEventArgs args) {
            createInterfaces(View.CollectionSource);
        }


        private void createInterfaces(CollectionSourceBase collectionSourceBase) {
            var iface = ((IInterfaceInfo) ObjectSpace.CreateObject(View.ObjectTypeInfo.Type));
            ObjectSpace.Session.Delete(iface);
            var assemblyName = iface.GetPropertyInfo(x=>x.Assembly).Name;
            var name = iface.GetPropertyInfo(x=>x.Name).Name;
            foreach (Type type in getInterfaces()) {
                if (ObjectSpace.Session.FindObject(View.ObjectTypeInfo.Type, CriteriaOperator.Parse(string.Format("{0}=? AND {1}=?", assemblyName, name), new AssemblyName(type.Assembly.FullName + "").Name, type.FullName)) == null){
                    createInterfaceInfo(type, collectionSourceBase);
                }
            }
            ObjectSpace.CommitChanges();
        }

        private void createInterfaceInfo(Type type, CollectionSourceBase collectionSourceBase) {
            var info= (IInterfaceInfo) ObjectSpace.CreateObject(View.ObjectTypeInfo.Type);
            info.Name = type.FullName;
            info.Assembly = new AssemblyName(type.Assembly.FullName + "").Name;
            collectionSourceBase.Add(info);
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
