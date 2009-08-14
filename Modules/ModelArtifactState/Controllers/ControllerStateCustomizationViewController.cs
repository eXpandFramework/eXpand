using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelArtifactState.Interfaces;
using eXpand.ExpressApp.ModelArtifactState.StateInfos;
using eXpand.ExpressApp.ModelArtifactState.StateRules;
using eXpand.ExpressApp.Core;

namespace eXpand.ExpressApp.ModelArtifactState.Controllers{
    public partial class ControllerStateCustomizationViewController : ViewController,ISupportArtifactStateVisibilityCustomization{
        public ControllerStateCustomizationViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            
            var controller = Frame.GetController<ArtifactStateCustomizationViewController>();
            controller.Register<ControllerStateRule>(this);
            
        }



        public void CustomizeVisibility(ArtifactStateInfo artifactStateInfo){
            var controllerStateRule = (((ControllerStateRule)artifactStateInfo.Rule));
            if (!string.IsNullOrEmpty(controllerStateRule.Module)){
                IEnumerable<string> assemblies = Application.Modules.Where(
                    @base => new Regex(controllerStateRule.Module).IsMatch(@base.GetType().FullName)).Select(
                    @base => @base.GetType().Assembly.FullName);
                foreach (
                    Controller controller in
                        Frame.Controllers.Cast<Controller>().Where(
                            controller => assemblies.Contains(controller.GetType().Assembly.FullName)))
                    controller.Active[ArtifactStateCustomizationViewController.ActiveKeyObjectTypeHasRules] =
                        !artifactStateInfo.Active;
            }
            else
                Frame.GetController(controllerStateRule.ControllerType).Active[
                    ArtifactStateCustomizationViewController.ActiveKeyObjectTypeHasRules] = !artifactStateInfo.Active;
           
        }
    }
}