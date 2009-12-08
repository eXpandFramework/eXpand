using System;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp.SystemModule
{
    public partial class ModuleController : Controller
    {
        public const string Modules = "Modules";
        public ModuleController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        public override void UpdateModel(Dictionary dictionary)
        {
            base.UpdateModel(dictionary);
            var wrapper = new ApplicationNodeWrapper(dictionary);
            DictionaryNode node = wrapper.Node.AddChildNode(Modules);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try{
                    foreach (var type in assembly.GetTypes().Where(type => typeof(ModuleBase).IsAssignableFrom(type))){
                        if (node.FindChildNode("Module", "Name", type.FullName) == null)
                            node.AddChildNode("Module").SetAttribute("Name", type.FullName);
                    }
                }
                catch (ReflectionTypeLoadException){
                    Tracing.Tracer.LogError(string.Format("ReflectionTypeLoadException for {0}", assembly.FullName));
                }
            }
        }

        [CoverageExclude]
        public override Schema GetSchema()
        {
            return new Schema(new DictionaryXmlReader().ReadFromString(
                                  @"<?xml version=""1.0""?>" +
                                  @"<Element Name=""Application"">" +
                                  @"	<Element Name=""Modules"">" +                                
                                  @"	    <Element Name=""Module"" KeyAttribute=""Name"" DisplayAttribute=""Name"" Multiple=""True"">" +                                
                                  @"	        <Attribute Name=""Name"" />" +                                
                                  @"	    </Element>" +
                                  @"	</Element>" +
                                  @"</Element>"));
        }
    }
}
