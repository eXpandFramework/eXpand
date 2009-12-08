using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using eXpand.ExpressApp.ModelArtifactState.Interfaces;
using eXpand.ExpressApp.ModelArtifactState.StateInfos;
using eXpand.ExpressApp.ModelArtifactState.StateRules;

namespace eXpand.ExpressApp.ModelArtifactState.Controllers{
    public partial class ActionStateCustomizationViewController : ViewController, ISupportArtifactStateVisibilityCustomization, ISupportArtifactStateAccessibilityCustomization
    {
        public ActionStateCustomizationViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }
        protected override void OnFrameAssigned()
        {
            base.OnFrameAssigned();
            var controller = Frame.GetController<ArtifactStateCustomizationViewController>();
            controller.Register<ActionStateRule>(this);

        }

        public void CustomizeVisibility(ArtifactStateInfo artifactStateInfo){
            var rule = (((ActionStateRule)artifactStateInfo.Rule));
            foreach (ActionBase actionBase in GetActions(rule)){
                actionBase.Active[ArtifactStateCustomizationViewController.ActiveObjectTypeHasRules] =
                    !artifactStateInfo.Active;
            }
            
        }
        private IEnumerable<ActionBase> GetActions(ActionStateRule rule)
        {
            IEnumerable<ActionBase> actionBases =
                Frame.Controllers.Cast<Controller>().SelectMany(controller => controller.Actions);
            if (!string.IsNullOrEmpty(rule.Module))
            {
                IEnumerable<string> assemblies =
                    Application.Modules.Where(@base => new Regex(rule.Module).IsMatch(@base.GetType().FullName)).Select(
                        @base => @base.GetType().Assembly.FullName);
                return actionBases.Where(@base => assemblies.Contains(@base.Controller.GetType().Assembly.FullName));
                
            }
            ActionBase actionBase =
                actionBases.Where(
                    @base => @base.Id == rule.ActionId).Select(@base => @base)
                    .Single();
            return new List<ActionBase>{actionBase};
        }
        public void CustomizeAccessibility(ArtifactStateInfo artifactStateInfo){
            var rule = (((ActionStateRule)artifactStateInfo.Rule));
            foreach (ActionBase actionBase in GetActions(rule))
            {
                actionBase.Enabled[ArtifactStateCustomizationViewController.ActiveObjectTypeHasRules] =
                    !artifactStateInfo.Active;
            }

        }
    }
}