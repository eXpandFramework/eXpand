using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;

namespace Xpand.ExpressApp.SystemModule
{
    public abstract class ListViewController<TListEditor> : ViewController<XpandListView> where TListEditor : ListEditor
    {
        protected override void OnViewChanging(View view)
        {
            base.OnViewChanging(view);
            if (View !=null){
                View.EditorChanged -= listView_EditorChanged;
            }
            if (view is XpandListView){
                ((XpandListView)view).EditorChanged += listView_EditorChanged;
                UpdateActiveState((view as XpandListView).Model.EditorType);
            }
        }

        private void listView_EditorChanged(object sender, EventArgs e) {
            var listEditor = ((XpandListView)sender).Editor;
            if (listEditor != null) UpdateActiveState(listEditor.GetType());
        }

        private void UpdateActiveState(Type editorType)
        {
            Active["Editor is not " + typeof(TListEditor).Name] = typeof(TListEditor).IsAssignableFrom(editorType);
        }
    }
}