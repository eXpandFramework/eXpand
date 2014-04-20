using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.Xpo;

namespace Xpand.ExpressApp.SystemModule.Search {
    [ModelAbstractClass]
    public interface IModelMemberFullTestContains:IModelMember{
        bool FullText { get; set; }
    }

    public class FullTextContainsController:ViewController<ListView>,IModelExtender {
        protected override void OnActivated(){
            base.OnActivated();
            View.CollectionSource.CriteriaApplying+=CollectionSourceOnCriteriaApplying;
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            View.CollectionSource.CriteriaApplying-=CollectionSourceOnCriteriaApplying;
        }

        private void CollectionSourceOnCriteriaApplying(object sender, EventArgs eventArgs){
            var collectionSourceBase = View.CollectionSource;
            collectionSourceBase.BeginUpdateCriteria();
            var criteriaOperators = collectionSourceBase.Criteria;
            var names = new HashSet<string>(View.Model.Columns.Select(column => column.ModelMember).Where(info => info.MemberInfo != null).Cast<IModelMemberFullTestContains>().Where(contains => 
                contains.FullText).Select(contains => contains.MemberInfo.Name));
            foreach (var criteriaOperator in criteriaOperators){
                new FullTextOperatorProcessor(names).Process(criteriaOperator);    
            }
            collectionSourceBase.EndUpdateCriteria();
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelMember,IModelMemberFullTestContains>();
        }
    }

}
