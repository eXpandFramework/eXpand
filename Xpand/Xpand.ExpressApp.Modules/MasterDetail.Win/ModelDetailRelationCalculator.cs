using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using Xpand.ExpressApp.MasterDetail.Logic;

namespace Xpand.ExpressApp.MasterDetail.Win {
    public class ModelDetailRelationCalculator {
        readonly IModelListView _modelListView;
        readonly XafGridView _xafGridView;
        readonly List<MasterDetailRuleInfo> _masterDetailRules;

        public ModelDetailRelationCalculator(IModelListView modelListView, XafGridView xafGridView, List<MasterDetailRuleInfo> masterDetailRules) {
            _modelListView = modelListView;
            _xafGridView = xafGridView;
            _masterDetailRules = masterDetailRules;
        }

        public bool IsRelationSet(int rowHandle, int relationIndex) {
            string rName = _xafGridView.GetRelationName(rowHandle, relationIndex);
            return GetMasterDetailRule(rName) != null;
        }

        MasterDetailRuleInfo GetMasterDetailRule(string rName) {
            return _masterDetailRules.LastOrDefault(rule => rule.CollectionMember.Name == rName);
        }

        public IModelListView GetChildModelListView(int rowHandle, int relationIndex) {
            var rName = _xafGridView.GetRelationName(rowHandle, relationIndex);
            return GetMasterDetailRule(rName).ChildListView;
        }

        public string GetOwnerPropertyName(IModelListView childModelListView, int rowHandle, int relationIndex) {
            var rName = _xafGridView.GetRelationName(rowHandle, relationIndex);
            IMemberInfo associatedMemberInfo = _modelListView.ModelClass.AllMembers.Single(member => member.MemberInfo.Name == rName).MemberInfo.AssociatedMemberInfo;
            return associatedMemberInfo.Name;

        }

        public IModelMember GetRelationModelMember(int rowHandle, int relationIndex) {

            var relationName = _xafGridView.GetRelationName(rowHandle, relationIndex);
            return _modelListView.ModelClass.AllMembers.Single(member => member.Name == relationName);
        }

    }
}