using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.TypeConverters;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.MasterDetail;

namespace Xpand.ExpressApp.MasterDetail.Logic {
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