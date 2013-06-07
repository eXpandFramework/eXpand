using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.SystemModule;
using System.Linq;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class SearchFromDetailViewController : ExpressApp.SystemModule.Search.SearchFromDetailViewController {
        public SearchFromDetailViewController() {
            SearchAction.Execute += SearchActionOnExecute;
        }

        void SearchActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            var propertyEditor = ((DetailView)View).GetItems<PropertyEditor>().Single(editor => editor.PropertyName == SearchAbleMemberInfos.ToList()[0].Name);
            ((Control)propertyEditor.Control).Focus();
        }

        protected override void ChangeObject(object findObject) {
            Frame.GetController<WinModificationsController>().ModificationsHandlingMode = ModificationsHandlingMode.AutoCommit;
            base.ChangeObject(findObject);
            Frame.GetController<WinModificationsController>().ModificationsHandlingMode = ModificationsHandlingMode.Confirmation;
        }
    }
}