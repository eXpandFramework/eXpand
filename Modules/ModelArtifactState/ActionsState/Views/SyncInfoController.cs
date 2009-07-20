using eXpand.ExpressApp.ModelArtifactState.Controllers;

namespace eXpand.ExpressApp.ModelArtifactState.ActionsState.Views
{
    public partial class SyncInfoController : ViewsSyncInfoController
    {
        public const string ConditionalActionState = "ConditionalActionState";
        public SyncInfoController()
        {
            InitializeComponent();
            RegisterActions(components);
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