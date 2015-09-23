using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.SystemModule.Filtering {
    public class ApplyFilterCriteriaToListViewController : ViewController<ListView>{
        private readonly SingleChoiceAction _filteringCriteria;

        public ApplyFilterCriteriaToListViewController() {
            _filteringCriteria = new SingleChoiceAction(this, "FilteringCriteria", PredefinedCategory.Filters);
            _filteringCriteria.Execute += FilteringCriteria_Execute;
        }

        public SingleChoiceAction FilteringCriteria {
            get { return _filteringCriteria; }
        }

        private void FilteringCriteria_Execute(object sender, SingleChoiceActionExecuteEventArgs e) {
            var collectionSource = View.CollectionSource;
            collectionSource.BeginUpdateCriteria();
            collectionSource.Criteria.Clear();
            collectionSource.Criteria[e.SelectedChoiceActionItem.Caption] = CriteriaEditorHelper.GetCriteriaOperator(e.SelectedChoiceActionItem.Data as string, View.ObjectTypeInfo.Type, ObjectSpace);
            collectionSource.EndUpdateCriteria();
        }

        protected override void OnActivated() {
            base.OnActivated();
            _filteringCriteria.Items.Clear();
            foreach (var criterion in ObjectSpace.GetObjects<FilteringCriteria>()) {
                if (criterion.ObjectType.IsAssignableFrom(View.ObjectTypeInfo.Type)) {
                    _filteringCriteria.Items.Add(new ChoiceActionItem(criterion.Name, criterion.Criteria));
                }
            }

            if (_filteringCriteria.Items.Count > 0)
                _filteringCriteria.Items.Add(new ChoiceActionItem("All", null));
        }
    }
}