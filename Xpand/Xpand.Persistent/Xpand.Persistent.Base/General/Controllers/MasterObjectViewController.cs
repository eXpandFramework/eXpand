using System;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.General.Controllers {
    public class FilteredMasterObjectViewController:MasterObjectViewController<object,object>{
        private string _criteria;

        protected override void OnActivated(){
            var collectionSource = View.CollectionSource as PropertyCollectionSource;
            var criteriaAttribute = collectionSource?.MemberInfo.FindAttribute<DataSourceCriteriaAttribute>();
            if (criteriaAttribute != null){
                _criteria = criteriaAttribute.DataSourceCriteria;
            }
            base.OnActivated();
        }

        protected override void UpdateMasterObject(object masterObject){
            if (_criteria != null){
                CriteriaWrapper criteriaWrapper = new CriteriaWrapper(_criteria, masterObject);
                criteriaWrapper.UpdateParametersValues(masterObject);
                ((PropertyCollectionSource) View.CollectionSource).Criteria[nameof(FilteredMasterObjectViewController)]= criteriaWrapper.CriteriaOperator;
            }
        }
    }
    public abstract class MasterObjectViewController<TNestedObject, TMasterObject> : ViewController<ListView> where TMasterObject : class{
        protected MasterObjectViewController() {
            TargetViewNesting = Nesting.Nested;
            TargetObjectType = typeof(TNestedObject);
        }
        protected override void OnActivated() {
            base.OnActivated();
            if (View.CollectionSource is PropertyCollectionSource collectionSource) {
                collectionSource.MasterObjectChanged += OnMasterObjectChanged;
                if (collectionSource.MasterObject is TMasterObject masterObject)
                    UpdateMasterObject(masterObject);
            }
        }

        protected abstract void UpdateMasterObject(TMasterObject masterObject);

        void OnMasterObjectChanged(object sender, EventArgs e){
            if (((PropertyCollectionSource)sender).MasterObject is TMasterObject masterObject){
	            UpdateMasterObject(masterObject);
	        }
        }

        protected override void OnDeactivated() {
            if (View.CollectionSource is PropertyCollectionSource collectionSource) {
                collectionSource.MasterObjectChanged -= OnMasterObjectChanged;
            }
            base.OnDeactivated();
        }
    }
}