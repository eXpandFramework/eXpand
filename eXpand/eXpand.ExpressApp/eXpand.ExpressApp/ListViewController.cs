using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;

namespace eXpand.ExpressApp {
    public abstract class ListViewController<TListEditor>:ViewController<ListView> where TListEditor:ListEditor{
        protected override void OnActivated()
        {
            base.OnActivated();
            Active["Editor is not " + typeof (TListEditor).Name] =typeof (TListEditor).IsAssignableFrom(View.Model.EditorType);
        }
    }
}