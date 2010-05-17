using System;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.Thumbnail.Web {
    public class RefreshThumbnailListEditorController : ViewController {
        public RefreshThumbnailListEditorController() {
            TargetViewType = ViewType.ListView;
            TargetViewNesting = Nesting.Any;
        }

        void CollectionSource_CriteriaApplied(object sender, EventArgs e) {
            var listEditor = ((ListView) View).Editor as ThumbnailListEditor;
            if (listEditor != null) {
                listEditor.Refresh();
            }
        }

        protected override void OnActivated() {
            base.OnActivated();
            var listEditor = ((ListView) View).Editor as ThumbnailListEditor;
            if (listEditor != null) {
                ((ListView) View).CollectionSource.CriteriaApplied += CollectionSource_CriteriaApplied;
            }
        }

        protected override void OnDeactivating() {
            var listEditor = ((ListView) View).Editor as ThumbnailListEditor;
            if (listEditor != null) {
                ((ListView) View).CollectionSource.CriteriaApplied -= CollectionSource_CriteriaApplied;
            }
            base.OnDeactivating();
        }
    }
}