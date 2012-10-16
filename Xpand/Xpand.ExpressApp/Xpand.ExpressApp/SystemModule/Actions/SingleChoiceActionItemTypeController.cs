using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.SystemModule.Actions {
    [ModelAbstractClass]
    public interface IModelActionSingleChoiceActionItemType : IModelAction {
        [Category("Appearance")]
        SingleChoiceActionItemType? ItemType { get; set; }
    }

    public class SingleChoiceActionItemTypeController : ViewController, IModelExtender {
        protected override void OnActivated() {
            base.OnActivated();
            var modelActions = Application.Model.ActionDesign.Actions.OfType<IModelActionSingleChoiceActionItemType>().Where(type => type.ItemType.HasValue);
            var singleChoiceActions = SingleChoiceActions();
            foreach (var modelAction in modelActions) {
                if (modelAction.ItemType != null)
                    singleChoiceActions[modelAction.Id].ItemType = modelAction.ItemType.Value;
            }
        }

        Dictionary<string, SingleChoiceAction> SingleChoiceActions() {
            var singleChoiceActions = Frame.Controllers.Cast<Controller>().SelectMany(controller => controller.Actions).OfType<SingleChoiceAction>();
            var choiceActions = new Dictionary<string, SingleChoiceAction>();
            foreach (var singleChoiceAction in singleChoiceActions.Where(singleChoiceAction => !choiceActions.ContainsKey(singleChoiceAction.Id))) {
                choiceActions.Add(singleChoiceAction.Id, singleChoiceAction);
            }
            return choiceActions;
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelAction, IModelActionSingleChoiceActionItemType>();
        }
    }
}
