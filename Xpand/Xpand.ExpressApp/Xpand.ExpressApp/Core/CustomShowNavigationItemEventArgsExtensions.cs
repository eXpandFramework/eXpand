using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.Core
{
    public static class CustomShowNavigationItemEventArgsExtensions
    {
        public static bool FitToObjectType(this CustomShowNavigationItemEventArgs args, XafApplication application,Type objectType)
        {
            var viewShortcut = args.ActionArguments.SelectedChoiceActionItem.Data as ViewShortcut;
            if (viewShortcut != null) {
                var view = application.Model.Views[viewShortcut.ViewId] as IModelObjectView;
                return view != null && view.ModelClass.TypeInfo.FullName == objectType.FullName;
            }

            return false;
        }
    }
}
