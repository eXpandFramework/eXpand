using eXpand.ExpressApp.ModelArtifactState.Controllers;

namespace eXpand.ExpressApp.ModelArtifactState.ControllersState.Views
{
    public partial class SyncInfoController : ViewsSyncInfoController
    {
        public const string ConditionalControlllerState = "ConditionalControlllerState";
        public SyncInfoController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override string GetElementStateActionName()
        {
            return ConditionalControlllerState;
        }

        protected override string GetMoreSchema()
        {
            return @"<Attribute Name=""" + STR_Name + @""" IsReadOnly=""true"" IsLocalized=""False"" RefNodeName=""/Application/ActionDesign/Controllers/*"" 
                                        Required=""True""/>";
        }


    }
}