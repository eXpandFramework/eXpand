using DevExpress.ExpressApp;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using eXpand.ExpressApp.ModelArtifactState.Attributes;
using eXpand.ExpressApp.ModelArtifactState.Interfaces;
using eXpand.ExpressApp.ModelArtifactState.NodeWrappers;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.ModelArtifactState.Controllers
{
    public abstract partial class ExtendSchemaModelController : WindowController
    {
        protected const string STR_Name = "Name";

        protected ExtendSchemaModelController()
        {
            InitializeComponent();
            RegisterActions(components);
        }
        
        public override Schema GetSchema()
        {
            var schemaHelper = new SchemaHelper();
            schemaHelper.AttibuteCreating += (sender, args) => modifyAttributes(args);
            string CommonTypeInfos = @"<Element Name=""" + ModelArtifactStateNodeWrapper.NodeNameAttribute + @""">
                                            <Element Name=""" + GetElementStateGroupNodeName() + @""">
                                                <Element Name=""" + GetElementStateNodeName() + @""" KeyAttribute=""ID"" Multiple=""True"">
                                                    " + schemaHelper.Serialize<IModelRule>(true) + @"
                                                    " + schemaHelper.Serialize<IArtifactRule>(true) + @"
                                                    " + GetMoreSchema() + @"
				                                </Element>
                                            </Element>
                                        </Element>";
            var schema = new Schema(schemaHelper.Inject(CommonTypeInfos, ModelElement.Application));
            return schema;

        }

        private void modifyAttributes(AttibuteCreatedEventArgs args){
            if (args.Attribute.IndexOf("Nesting") > -1 || args.Attribute.IndexOf("ViewType") > -1)
                args.AddTag(@" IsInvisible=""{" + typeof (ViewVisibilityCalculator).FullName +
                            @"}ID=..\..\@ID;ViewType=" + ViewType.Any+@"""");
            else if (args.Attribute.IndexOf("Module") > -1)
                args.AddTag(@"AllowCustom=""True"" RefNodeName=""/Application/Modules/*""");
            else if (args.Attribute.IndexOf("ID") > -1)
                args.AddTag(@" Required=""True""");
            else if (args.Attribute.IndexOf("TypeInfo") > -1)
                args.AddTag(@"Required=""True"" RefNodeName=""/Application/BOModel/Class""");
        }

        protected abstract string GetElementStateNodeName();

        public abstract string GetMoreSchema();
        public abstract string GetElementStateGroupNodeName();
    }
}