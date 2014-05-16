using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo.Metadata;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Xpo;
using Xpand.Xpo.DB;

namespace Xpand.ExpressApp.SystemModule.Search {
    [ModelAbstractClass]
    public interface IModelMemberFullTextContains:IModelMember{
        [Category("eXpand")]
        [Description("Supported in eXpand ListEditors, CollectionSource.CriteriaApplying, CriteriaPropertyEditorEx, gridView.ColumnFilterChanged, XpandObjectSpaceProvider")]
        bool FullText { get; set; }
    }

    public static class ModelMemberFullTextContainsEx{
        public static IEnumerable<IModelMember> GetFullTextMembers(this IModelClass modelClass){
            return modelClass.AllMembers.Cast<IModelMemberFullTextContains>().Where(member => member.FullText);
        }
        public static IEnumerable<IModelMember> GetFullTextMembers(this IModelListView modelListView){
            return modelListView.Columns.Select(column => column.ModelMember).Where(info => info.MemberInfo != null).Cast<IModelMemberFullTextContains>().Where(contains => contains.FullText);
        }
    }
    public class FullTextContainsController:ViewController<ListView>,IModelExtender {
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
            var xpDictionary = Application.GetXPDictionary();
            var dictionary = new Dictionary<string, List<XPMemberInfo>>();
            foreach (var fullTextMember in fullTextMembers){
                var tableName = xpDictionary.GetClassInfo(fullTextMember.ModelClass.TypeInfo.Type).TableName;
                if (!dictionary.ContainsKey(tableName))
                    dictionary.Add(tableName,new List<XPMemberInfo>());
                var memberInfos = dictionary[tableName];
                if (memberInfos.All(info => info.Name != fullTextMember.Name))
                    memberInfos.Add(fullTextMember.GetXpmemberInfo());
            }
            return dictionary;
        }

        private void ProxyOnDataStoreSelectData(object sender, DataStoreSelectDataEventArgs dataStoreSelectDataEventArgs){
            var selectStatements = dataStoreSelectDataEventArgs.SelectStatements.Where(statement => _dictionary.ContainsKey(statement.TableName));
            foreach (var selectStatement in selectStatements){
                FullTextOperatorProcessor.Process(selectStatement.Condition, _dictionary[selectStatement.TableName]);
            }
        }

        private void CollectionSourceOnCriteriaApplying(object sender, EventArgs eventArgs){
            var collectionSourceBase = View.CollectionSource;
            collectionSourceBase.BeginUpdateCriteria();
            var memberInfos = View.Model.GetFullTextMembers().Select(member => member.GetXpmemberInfo());
            foreach (var criteriaOperator in collectionSourceBase.Criteria) {
                if (!ReferenceEquals(criteriaOperator,null)) FullTextOperatorProcessor.Process(criteriaOperator, memberInfos.ToList());
            }
            collectionSourceBase.EndUpdateCriteria();
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelMember,IModelMemberFullTextContains>();
        }
    }

}
