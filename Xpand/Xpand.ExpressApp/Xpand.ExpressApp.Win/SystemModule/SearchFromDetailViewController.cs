using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.SystemModule;
using System.Linq;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class SearchFromDetailViewController : ExpressApp.SystemModule.SearchFromDetailViewController
    {
        public SearchFromDetailViewController() {
            SearchAction.Execute+=SearchActionOnExecute;
        }

        void SearchActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            var propertyEditor = ((XpandDetailView)View).GetItems<PropertyEditor>().Where(editor => editor.PropertyName == SearchAbleMemberInfos.ToList()[0].Name).Single();
            ((Control)propertyEditor.Control).Focus();            
        }

        protected override void ChangeObject(object findObject)
        {
            Frame.GetController<WinDetailViewController>().SuppressConfirmation = true;
            base.ChangeObject(findObject);
            Frame.GetController<WinDetailViewController>().SuppressConfirmation = false;
        }
    }
}