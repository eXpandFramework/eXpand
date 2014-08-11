using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using Fasterflect;
using System.Linq;

namespace Xpand.Persistent.Base.General {
    public static class FrameExtensions {
        public static IEnumerable<TAction> Actions<TAction>(this Frame frame, IEnumerable<IModelNode> items) where TAction : ActionBase {
            var choiceActions = frame.Actions<TAction>().ToDictionary(action => action.Id, action => action);
            return choiceActions.Any() ? items.GroupBy(model => model.GetParent<IModelAction>()).Where(nodes => choiceActions.ContainsKey(nodes.Key.Id)).Select(@group => choiceActions[@group.Key.Id]).Distinct() : Enumerable.Empty<TAction>();
        }

        public static IEnumerable<ActionBase> Actions(this Frame frame){
            return frame.Actions<ActionBase>();
        }

        public static IEnumerable<T> Actions<T>(this Frame frame) where T:ActionBase{
            return frame.Controllers.Cast<Controller>().SelectMany(controller => controller.Actions).OfType<T>();
        }

        public static void OnViewChanged(this Frame frame){
            frame.CallMethod("OnViewChanged", frame);
        }

        public static IEnumerable<T> GetControllers<T>(this Frame frame) where T:Controller {
// ReSharper disable once RedundantEnumerableCastCall
            return frame.Controllers.Cast<Controller>().OfType<T>();
        }

        public static Controller GetController(this Frame frame, Type controllerType){
            return (Controller)frame.CallMethod(new[] { controllerType }, "GetController");
        }
    }
}
