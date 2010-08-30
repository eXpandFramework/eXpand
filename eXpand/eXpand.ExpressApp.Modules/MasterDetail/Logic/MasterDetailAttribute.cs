using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.Logic.Conditional.Logic;
using eXpand.ExpressApp.Logic.TypeConverters;

namespace eXpand.ExpressApp.MasterDetail.Logic {
    public class MasterDetailAttribute : ConditionalLogicRuleAttribute, IMasterDetailRule
    {
        public MasterDetailAttribute(string id, string normalCriteria, string childListView, string collectionMember) : base(id, normalCriteria, String.Empty) {
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