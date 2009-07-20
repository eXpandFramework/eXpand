using eXpand.ExpressApp.ModelArtifactState.Parsers;
using eXpand.ExpressApp.Security.Controllers;

namespace eXpand.ExpressApp.ModelArtifactState.ActionsState.BOModel
{
    public partial class SyncInfoController : BOModelSyncInfoController
    {
        public const string ConditionalActionState = "ConditionalActionState";
        
        public SyncInfoController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            var parser = new ActionStateRulesNodeParser();
            parser.AddAttributesToXafTypesInfoFromBOModel(this,ConditionalActionState);
            
        }

        protected override string GetMoreSchema()
        {
            return @"<Attribute Name=""" + STR_Name + @""" IsReadOnly=""true"" IsLocalized=""False"" RefNodeName=""/Application/ActionDesign/Actions/Action"" 
                                        Required=""True""/>";
        }

        protected override string GetElementStateActionName()
        {
            return ConditionalActionState;
        }
    }
}