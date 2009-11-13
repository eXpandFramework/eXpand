using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.Core
{
    public static class CustomShowNavigationItemEventArgsExtensions
    {
        public static bool FitToObjectType(this CustomShowNavigationItemEventArgs args, XafApplication application,Type objectType)
        {
            var viewShortcut = args.ActionArguments.SelectedChoiceActionItem.Data as ViewShortcut;
            if (viewShortcut != null &&
                new ApplicationNodeWrapper(application.Info).Views.FindViewById(viewShortcut.ViewId).ClassName ==
                objectType.FullName)
                return true;
            return false;
        }
    }
}
