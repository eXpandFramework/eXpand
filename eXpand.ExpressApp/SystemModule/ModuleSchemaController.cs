using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.SystemModule
{
    public partial class ModuleSchemaController : Controller
    {
        public const string ModulesDesign = "ModulesDesign";
        public ModuleSchemaController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

//        protected override void OnActivated()
//        {
//            base.OnActivated();
//            DictionaryNode childNode = Application.Info.GetChildNode(ModulesDesign);
//            foreach (var module in Application.Modules)
//                childNode.AddChildNode(new DictionaryNode(module.Name));
//        }

        public override Schema GetSchema()
        {
            const string CommonTypeInfos = @"<Element Name=""Application"">
                        <Element Name=""" + ModulesDesign + @""" Multiple=""False"">
                            
                        </Element>
                  </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(CommonTypeInfos));
        }
    }
}