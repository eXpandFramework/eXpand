using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.Core
{
    public static class CustomShowNavigationItemEventArgsExtensions
    {
        public static bool FitToObjectType(this CustomShowNavigationItemEventArgs args, XafApplication application,Type objectType)
        {
            var viewShortcut = args.ActionArguments.SelectedChoiceActionItem.Data as ViewShortcut;
            if (viewShortcut != null && 
                application.Model.Views[viewShortcut.ViewId].ModelClass.TypeInfo.FullName == objectType.FullName)
                return true;
            return false;
        }
    }
}
