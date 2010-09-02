using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;

namespace eXpand.ExpressApp.SystemModule
{
    public abstract class ListViewController<TListEditor> : ViewController<ListView> where TListEditor : ListEditor
    {
        protected override void OnViewChanging(View view)
        {
            base.OnViewChanging(view);
            if (View is ListView)
            {
                ((ListView)View).EditorChanged -= listView_EditorChanged;
            }
            if (view is ListView)
            {
                ((ListView)view).EditorChanged += listView_EditorChanged;
                UpdateActiveState((view as ListView).Model.EditorType);
            }
        }

        private void listView_EditorChanged(object sender, System.EventArgs e)
        {
            UpdateActiveState(((ListView)sender).Editor.GetType());
        }

        private void UpdateActiveState(Type editorType)
        {
            Active["Editor is not " + typeof(TListEditor).Name] = typeof(TListEditor).IsAssignableFrom(editorType);
        }
    }
}