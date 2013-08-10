using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;

namespace Xpand.ExpressApp.SystemModule {
    public abstract class ListViewController<TListEditor> : ViewController<ListView> where TListEditor : ListEditor {
        protected override void OnViewChanging(View view) {
            base.OnViewChanging(view);
            if (View != null) {
                View.EditorChanged -= listView_EditorChanged;
            }
            var listView = view as ListView;
            if (listView != null) {
                listView.EditorChanged += listView_EditorChanged;
                UpdateActiveState(listView.Model.EditorType);
            }
        }

        private void listView_EditorChanged(object sender, EventArgs e) {
            var listEditor = ((ListView)sender).Editor;
            if (listEditor != null) UpdateActiveState(listEditor.GetType());
        }

        private void UpdateActiveState(Type editorType) {
            Active["Editor is not " + typeof(TListEditor).Name] = typeof(TListEditor).IsAssignableFrom(editorType);
        }
    }
}