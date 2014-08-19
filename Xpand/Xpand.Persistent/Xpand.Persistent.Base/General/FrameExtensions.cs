using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using Fasterflect;
using System.Linq;
using Xpand.Utils.Linq;

namespace Xpand.Persistent.Base.General {
    public static class FrameExtensions {
        public static IEnumerable<TAction> Actions<TAction>(this Frame frame, IEnumerable<IModelNode> items) where TAction : ActionBase {
            var list = items.DistinctBy(model => model.GetParent<IModelAction>()).ToList();
            return frame.Actions<TAction>().Where(choiceAction => list.Contains(choiceAction.Model));
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
