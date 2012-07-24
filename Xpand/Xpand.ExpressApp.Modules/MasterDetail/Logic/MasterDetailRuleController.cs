using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.Conditional.Logic;
using Xpand.ExpressApp.Logic.Model;
using Xpand.ExpressApp.MasterDetail.Model;
using Xpand.Utils.Linq;

namespace Xpand.ExpressApp.MasterDetail.Logic {
    public class MasterDetailRuleInfo : IEqualityComparer {
        public MasterDetailRuleInfo(IModelListView childListView, IModelMember collectionMember, ITypeInfo typeInfo) {
            ChildListView = childListView;
            CollectionMember = collectionMember;
            TypeInfo = typeInfo;
        }

        public IModelListView ChildListView { get; set; }
        public IModelMember CollectionMember { get; set; }
        public ITypeInfo TypeInfo { get; set; }

        bool IEqualityComparer.Equals(object x, object y) {
            throw new NotImplementedException();
        }

        public int GetHashCode(object obj) {
            throw new NotImplementedException();
        }
    }
    public class MasterDetailRuleController : ConditionalLogicRuleViewController<IMasterDetailRule> {
        readonly List<IMasterDetailRule> _masterDetailRules = new List<IMasterDetailRule>();

        protected override IModelLogic GetModelLogic() {
            return ((IModelApplicationMasterDetail)Application.Model).MasterDetail;
        }

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            var masterDetailViewControllerBase = Frame.GetController<MasterDetailViewControllerBase>();
            masterDetailViewControllerBase.RequestRules = frame1 => {
                var masterDetailRules = frame1.GetController<MasterDetailRuleController>()._masterDetailRules.DistinctBy(rule => rule.Id);
                return masterDetailRules.Select(rule => new MasterDetailRuleInfo(rule.ChildListView, rule.CollectionMember, rule.TypeInfo)).ToList();
            };
        }

        public override void ExecuteRule(LogicRuleInfo<IMasterDetailRule> info, ExecutionContext executionContext) {
            if (info.Active) {
                if (!(_masterDetailRules.Contains(info.Rule))) {
                    _masterDetailRules.Add(info.Rule);
                }
            } else {
                _masterDetailRules.Remove(info.Rule);
            }
        }

        public List<IMasterDetailRule> MasterDetailRules {
            get { return _masterDetailRules; }
        }

    }

}