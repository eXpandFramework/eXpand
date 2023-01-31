using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.Extensions.XAF.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.Base.Xpo;
using Xpand.Xpo.DB;

namespace Xpand.ExpressApp.SystemModule.Search {
    [ModelAbstractClass]
    public interface IModelMemberFullTextContains : IModelMember {
        [Category(AttributeCategoryNameProvider.Search)]
        [Description("Supported in eXpand ListEditors, CollectionSource.CriteriaApplying, CriteriaPropertyEditorEx, gridView.ColumnFilterChanged, XpandObjectSpaceProvider")]
        [ModelBrowsable(typeof(ModelMemberFullTextContainsVisibilityCalculator))]
        bool FullText { get; set; }
    }

    public class ModelMemberFullTextContainsVisibilityCalculator:IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName){
            var memberInfo = ((IModelMember) node).MemberInfo;
            return memberInfo != null && (memberInfo.MemberType == typeof(string)&&memberInfo.FindAttributes<SizeAttribute>().Any(attribute => attribute.Size==SizeAttribute.Unlimited));
        }
    }
    public static class ModelMemberFullTextContainsEx {
        public static IEnumerable<IModelMember> GetFullTextMembers(this IModelListView modelListView) {
            return modelListView.Columns.Select(column => column.ModelMember).Where(info => info?.MemberInfo != null).Cast<IModelMemberFullTextContains>().Where(contains => contains.FullText);
        }
    }

    public class FullTextContainsController : ViewController<ListView>, IModelExtender {
        private XpandObjectSpaceProvider _xpandObjectSpaceProvider;
        private Dictionary<string, List<XPMemberInfo>> _dictionary;

        protected override void OnActivated(){
            base.OnActivated();
            View.CollectionSource.CriteriaApplying+=CollectionSourceOnCriteriaApplying;
            _xpandObjectSpaceProvider = Application.ObjectSpaceProviders.OfType<XpandObjectSpaceProvider>().FirstOrDefault();
            if (_xpandObjectSpaceProvider != null){
                _dictionary=CreateDictionary();
                _xpandObjectSpaceProvider.DataStoreProvider.Proxy.DataStoreSelectData+=ProxyOnDataStoreSelectData;
            }
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            View.CollectionSource.CriteriaApplying-=CollectionSourceOnCriteriaApplying;
            if (_xpandObjectSpaceProvider != null)
                _xpandObjectSpaceProvider.DataStoreProvider.Proxy.DataStoreSelectData -= ProxyOnDataStoreSelectData;
        }

        private Dictionary<string, List<XPMemberInfo>> CreateDictionary(){
            var fullTextMembers = View.Model.GetFullTextMembers();
            var dictionary = new Dictionary<string, List<XPMemberInfo>>();
            foreach (var fullTextMember in fullTextMembers){
                var tableName = fullTextMember.ModelClass.TypeInfo.QueryXPClassInfo().TableName;
                if (!dictionary.ContainsKey(tableName))
                    dictionary.Add(tableName,new List<XPMemberInfo>());
                var memberInfos = dictionary[tableName];
                if (memberInfos.All(info => info.Name != fullTextMember.Name))
                    memberInfos.Add(fullTextMember.GetXPMemberInfo());
            }
            return dictionary;
        }

        private void ProxyOnDataStoreSelectData(object sender, DataStoreSelectDataEventArgs dataStoreSelectDataEventArgs){
            var selectStatements = dataStoreSelectDataEventArgs.SelectStatements.Where(statement => _dictionary.ContainsKey(statement.Table.Name));
            foreach (var selectStatement in selectStatements){
                FullTextOperatorProcessor.Process(selectStatement.Condition, _dictionary[selectStatement.Table.Name]);
            }
        }

        private bool _applying;
        private void CollectionSourceOnCriteriaApplying(object sender, EventArgs eventArgs){
            if (!_applying) {
                var memberInfos = View.Model.GetFullTextMembers().Select(member => member.GetXPMemberInfo()).ToArray();
                if (memberInfos.Any()){
                    var collectionSourceBase = View.CollectionSource;
                    collectionSourceBase.BeginUpdateCriteria();
                    foreach (var key in collectionSourceBase.Criteria.Keys){
                        var criteriaOperator = collectionSourceBase.Criteria[key];
                        if (!ReferenceEquals(criteriaOperator, null)){
                            _applying = true;
                            collectionSourceBase.Criteria[key] =
                                (CriteriaOperator) FullTextOperatorProcessor.Process(criteriaOperator, memberInfos);
                        }
                    }
                    collectionSourceBase.EndUpdateCriteria();
                }
            }
            _applying = false;
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelMember,IModelMemberFullTextContains>();
        }
    }

}
