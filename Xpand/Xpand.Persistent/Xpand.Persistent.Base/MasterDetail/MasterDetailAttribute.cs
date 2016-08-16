using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.TypeConverters;
using Xpand.Persistent.Base.Logic;

namespace Xpand.Persistent.Base.MasterDetail {
    public sealed class MasterDetailAttribute : LogicRuleAttribute, IContextMasterDetailRule {
        public MasterDetailAttribute(string id, string normalCriteria, string childListView, string collectionMember)
            : base(id, normalCriteria, String.Empty) {
            ChildListView = childListView;
            CollectionMember = collectionMember;
        }

        public string ChildListView { get; set; }
        [TypeConverter(typeof(StringToModelViewConverter))]
        IModelListView IMasterDetailRule.ChildListView { get; set; }
        public string CollectionMember { get; set; }

        [TypeConverter(typeof(StringToModelMemberConverter))]
        IModelMember IMasterDetailRule.CollectionMember { get; set; }

    }

}