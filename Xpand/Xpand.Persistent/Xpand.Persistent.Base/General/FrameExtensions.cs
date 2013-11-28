using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using Xpand.Utils.Helpers;
using System.Linq;

namespace Xpand.Persistent.Base.General {
    public static class FrameExtensions {
        public static IEnumerable<T> GetControllers<T>(this Frame frame, Type controllerType) where T:Controller {
            return frame.Controllers.OfType<T>();
        }

        public static Controller GetController(this Frame frame, Type controllerType) {
            var name = frame.GetMemberInfo(x => x.GetController<Controller>()).Name;
            var invoke = frame.GetType().GetMethod(name).MakeGenericMethod(new[] { controllerType }).Invoke(frame, null);
            return invoke as Controller;
        }
    }
}
