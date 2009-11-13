using System;
using DevExpress.ExpressApp;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.Core
{
    public static class FrameExtensions
    {
        public static Controller GetController(this Frame frame, Type controllerType)
        {
            var name = frame.GetMethodInfo(x=>x.GetController<Controller>()).Name;
            var invoke = frame.GetType().GetMethod(name).MakeGenericMethod(new[] {controllerType}).Invoke(frame, null);
            return invoke as Controller;
        }
    }
}
