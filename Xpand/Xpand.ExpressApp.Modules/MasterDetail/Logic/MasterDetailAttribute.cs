using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.TypeConverters;

namespace Xpand.ExpressApp.MasterDetail.Logic {
    public sealed class MasterDetailAttribute : LogicRuleAttribute, IMasterDetailRule {
        public MasterDetailAttribute(string id, string normalCriteria, string childListView, string collectionMember)
            : base(id, normalCriteria, String.Empty) {
            ChildListView = childListView;
            CollectionMember = collectionMember;
        }

        public string ChildListView { get; set; }
        [TypeConverter(typeof(StringToModelViewConverter))]
        IModelListView IMasterDetailRule.ChildListView { get; set; }
        public string CollectionMember { get; set; }

        public bool SynchronizeActions { get; set; }

        [TypeConverter(typeof(StringToModelMemberConverter))]
        IModelMember IMasterDetailRule.CollectionMember { get; set; }
    }

}