using eXpand.ExpressApp.ModelArtifactState.Parsers;
using eXpand.ExpressApp.Security.Controllers;

namespace eXpand.ExpressApp.ModelArtifactState.ControllersState.BOModel
{
    public partial class SyncInfoController : BOModelSyncInfoController
    {
        
        public const string ConditionalControlllerState = "ConditionalControlllerState";
        
        public SyncInfoController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            var parser = new ControllerStateAttributesNodeParser();
            parser.AddAttributesToXafTypesInfoFromBOModel(this,ConditionalControlllerState);
        }

        protected override string GetMoreSchema()
        {
            return @"<Attribute Name=""" + STR_Name + @""" IsReadOnly=""true"" IsLocalized=""False"" RefNodeName=""/Application/ActionDesign/Controllers/*"" 
                                        Required=""True""/>";
        }

        protected override string GetElementStateActionName()
        {
            return ConditionalControlllerState;
        }
    }
}