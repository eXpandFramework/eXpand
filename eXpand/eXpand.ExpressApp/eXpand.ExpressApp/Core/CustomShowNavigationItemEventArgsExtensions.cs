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
            var viewShortcut = args.ActionArguments.SelectedChoiceActionItem.Data as DevExpress.ExpressApp.ViewShortcut;
            if (viewShortcut != null && 
                application.Model.Views[viewShortcut.ViewId].ModelClass.TypeInfo.FullName == objectType.FullName)
                return true;
            return false;
        }
    }
}
