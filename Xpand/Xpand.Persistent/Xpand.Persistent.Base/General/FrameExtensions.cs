using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using Fasterflect;
using System.Linq;

namespace Xpand.Persistent.Base.General {
    public static class FrameExtensions {
        public static IEnumerable<TAction> Actions<TAction>(this Frame frame, IEnumerable<IModelNode> items) where TAction : ActionBase{
            var choiceActions = frame.Actions<TAction>().ToArray();
            if (choiceActions.Any())
                return items.GroupBy(model => model.GetParent<IModelAction>())
                    .Where(nodes => choiceActions.Select(action => action.Id).Any(s => s == nodes.Key.Id))
                    .Select(@group => choiceActions.First(action => action.Id == @group.Key.Id)).Distinct();
            return Enumerable.Empty<TAction>();
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
            foreach (var controller in frame.Controllers){
                if (controller is T)
                    yield return (T) controller;
            }
        }

        public static Controller GetController(this Frame frame, Type controllerType){
            return (Controller)frame.CallMethod(new[] { controllerType }, "GetController");
        }
    }
}
