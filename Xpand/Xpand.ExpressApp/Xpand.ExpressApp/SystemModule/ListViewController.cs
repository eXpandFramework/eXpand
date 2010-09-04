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
            if (View is XpandListView)
            {
                ((XpandListView)View).EditorChanged -= listView_EditorChanged;
            }
            if (view is XpandListView)
            {
                ((XpandListView)view).EditorChanged += listView_EditorChanged;
                UpdateActiveState((view as XpandListView).Model.EditorType);
            }
        }

        private void listView_EditorChanged(object sender, System.EventArgs e)
        {
            UpdateActiveState(((XpandListView)sender).Editor.GetType());
        }

        private void UpdateActiveState(Type editorType)
        {
            Active["Editor is not " + typeof(TListEditor).Name] = typeof(TListEditor).IsAssignableFrom(editorType);
        }
    }
}