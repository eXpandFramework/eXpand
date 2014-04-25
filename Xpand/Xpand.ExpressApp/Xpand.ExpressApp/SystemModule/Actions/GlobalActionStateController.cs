using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.SystemModule.Actions {
    [ModelAbstractClass]
    public interface IModelActionState:IModelAction {
        [DefaultValue(true)]
        [Category("eXpand")]
        bool Active { get; set; }
    }
    public class GlobalActionStateController:ViewController,IModelExtender {
        protected override void OnActivated() {
            base.OnActivated();
            var modelActions = new HashSet<string>(Application.Model.ActionDesign.Actions.Cast<IModelActionState>().Where(state => !state.Active).Select(state => state.Id));
            var actions = Frame.Controllers.Cast<Controller>().SelectMany(controller => controller.Actions).Where(@base => modelActions.Contains(@base.Id));
            foreach (var action in actions) {
                action.Active["ModelActiveAttribute"] = false;
            }
        }
        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelAction,IModelActionState>();
        }
    }
}
