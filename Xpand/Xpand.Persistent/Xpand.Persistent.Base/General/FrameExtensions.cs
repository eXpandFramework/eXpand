using System;
using DevExpress.ExpressApp;
using Xpand.Utils.Helpers;

namespace Xpand.Persistent.Base.General {
    public static class FrameExtensions {
        public static Controller GetController(this Frame frame, Type controllerType) {
            var name = frame.GetMemberInfo(x => x.GetController<Controller>()).Name;
            var invoke = frame.GetType().GetMethod(name).MakeGenericMethod(new[] { controllerType }).Invoke(frame, null);
            return invoke as Controller;
        }
    }
}
