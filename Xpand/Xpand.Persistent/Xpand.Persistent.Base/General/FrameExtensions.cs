using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using Fasterflect;
using System.Linq;
using DevExpress.ExpressApp.SystemModule;

namespace Xpand.Persistent.Base.General {
    public static class FrameExtensions {

        public static void CleanDetailView(this Frame frame) {
            frame.GetController<ModificationsController>(controller => {
                controller.Active["CleanDetailView"] = false;
                frame.View.CurrentObject = frame.View.ObjectSpace.CreateObject(frame.View.ObjectTypeInfo.Type);
                controller.Active.RemoveItem("CleanDetailView");
            });
        }

        public static IEnumerable<TAction> Actions<TAction>(this Frame frame, IEnumerable<IModelNode> items) where TAction : ActionBase{
            var choiceActions = frame.Actions<TAction>().ToArray();
            if (choiceActions.Any())
                return items.GroupBy(model => model.GetParent<IModelAction>())
                    .Where(nodes => choiceActions.Select(action => action.Id).Any(s => s == nodes.Key.Id))
                    .Select(group => choiceActions.First(action => action.Id == group.Key.Id)).Distinct();
            return Enumerable.Empty<TAction>();
        }

        public static bool Available(this ActionBase a){
            return a.Active && a.Enabled;
        }

        public static IEnumerable<ActionBase> Available(this IEnumerable<ActionBase> actionBases){
            return actionBases.Where(Available);
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

        public static IEnumerable<T> GetControllers<T>(this Frame frame) where T:class {
            foreach (var controller in frame.GetControllers(typeof(T))){
                if (controller is T controllers)
                    yield return controllers;
            }
        }

        public static void GetController<T>(this Frame frame, Action<T> withController) where T:Controller{
            var controller = frame?.GetController<T>();
            if (controller != null)
                withController(controller);
        }

        public static void GetController(this Frame frame, Type controllerType,Action<Controller> action){
            var controller = (Controller)frame.CallMethod(new[] { controllerType }, "GetController");
            if (controller!=null)
                action(controller);
        }

        public static Controller GetController(this Frame frame, Type controllerType){
            return (Controller)frame.CallMethod(new[] { controllerType }, "GetController");
        }
    }
}
