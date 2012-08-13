using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.SystemModule {
    [ModelAbstractClass]
    public interface IModelActionSingleChoiceActionItemType : IModelAction {
        [Category("Appearance")]
        SingleChoiceActionItemType? ItemType { get; set; }
    }

    public class SingleChoiceActionItemTypeController : ViewController, IModelExtender {
        protected override void OnActivated() {
            base.OnActivated();
            var modelActions = Application.Model.ActionDesign.Actions.OfType<IModelActionSingleChoiceActionItemType>().Where(type => type.ItemType.HasValue);
            var singleChoiceActions = Frame.Controllers.Cast<Controller>().SelectMany(controller => controller.Actions).OfType<SingleChoiceAction>().ToDictionary(action => action.Id, action => action);
            foreach (var modelAction in modelActions) {
                if (modelAction.ItemType != null)
                    singleChoiceActions[modelAction.Id].ItemType = modelAction.ItemType.Value;
            }
        }
        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelAction, IModelActionSingleChoiceActionItemType>();
        }
    }
}
