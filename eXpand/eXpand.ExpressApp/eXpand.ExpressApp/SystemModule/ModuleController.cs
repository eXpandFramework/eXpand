using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using eXpand.Utils.Helpers;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

namespace eXpand.ExpressApp.SystemModule
{
    public partial class ModuleController : Controller, IModelExtender
    {
        public ModuleController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelApplication, IModelModules>();
        }
    }

    [ModelNodesGenerator(typeof(ModuleNodesGenerator))]
    public interface IModelModules : IModelNode, IModelList<IModelModule>
    {
    }

    [KeyProperty("Name"), DisplayProperty("Name")]
    public interface IModelModule
    {
        string Name { get; set; }
    }

    public class ModuleNodesGenerator : ModelNodesGeneratorBase
    {
        protected override void GenerateNodesCore(ModelNode node)
        {
            IModelModules modules = node as IModelModules;
            foreach (var type in AppDomain.CurrentDomain.GetTypes(typeof(ModuleBase)))
            {
                if (node[type.FullName] == null)
                {
                    var module = node.AddNode<IModelModule>();
                    module.Name = type.FullName;
                }
            }
        }
    }
}
