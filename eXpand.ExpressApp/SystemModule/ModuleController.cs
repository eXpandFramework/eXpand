using System;
using System.Diagnostics;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;

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
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(type => typeof(ModuleBase).IsAssignableFrom(type));
            var wrapper = new ApplicationNodeWrapper(dictionary);
            var node = wrapper.Node.AddChildNode(Modules);
            foreach (Type type in types)
                if (node.FindChildNode("Module", "Name", type.FullName)== null)
                    node.AddChildNode("Module").SetAttribute("Name", type.FullName);
        }

        [DebuggerNonUserCode]
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
