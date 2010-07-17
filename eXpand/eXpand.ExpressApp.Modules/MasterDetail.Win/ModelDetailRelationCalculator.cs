using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using eXpand.ExpressApp.MasterDetail.Logic;

namespace eXpand.ExpressApp.MasterDetail.Win {
    public class ModelDetailRelationCalculator
    {
        readonly IModelListView _modelListView;
        readonly XafGridView _xafGridView;
        readonly List<IMasterDetailRule> _masterDetailRules;

        public ModelDetailRelationCalculator(IModelListView modelListView, XafGridView xafGridView, List<IMasterDetailRule> masterDetailRules) {
            _modelListView = modelListView;
            _xafGridView = xafGridView;
            _masterDetailRules = masterDetailRules;
        }

        public  bool IsRelationSet( int rowHandle, int relationIndex)
        {
            string rName = _xafGridView.GetRelationName(rowHandle, relationIndex);
            return GetMasterDetailRule(rName) != null;
        }

        IMasterDetailRule GetMasterDetailRule(string rName) {
            return _masterDetailRules.Where(rule => rule.CollectionMember.Name==rName).LastOrDefault();
        }

        public IModelListView GetChildModelListView(int rowHandle, int relationIndex)
        {
            var rName = _xafGridView.GetRelationName(rowHandle, relationIndex);
            return GetMasterDetailRule(rName).ChildListView;
        }

        public string GetOwnerPropertyName(IModelListView childModelListView,int rowHandle, int relationIndex) {
            var rName = _xafGridView.GetRelationName(rowHandle, relationIndex);
            IMemberInfo associatedMemberInfo = _modelListView.ModelClass.AllMembers.Where(member => member.MemberInfo.Name == rName).Single().MemberInfo.AssociatedMemberInfo;
            return associatedMemberInfo.Name;

        }
    }
}