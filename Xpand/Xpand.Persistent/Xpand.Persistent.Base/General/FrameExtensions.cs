using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using Fasterflect;
using System.Linq;

namespace Xpand.Persistent.Base.General {
    public static class FrameExtensions {

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
