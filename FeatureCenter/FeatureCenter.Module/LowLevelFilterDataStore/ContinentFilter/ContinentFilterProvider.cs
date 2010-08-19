using System;
using System.ComponentModel;
using DevExpress.Xpo;
using eXpand.ExpressApp.FilterDataStore.Core;

namespace FeatureCenter.Module.LowLevelFilterDataStore.ContinentFilter
{
    public class ContinentFilterProvider : FilterProviderBase
    {
        public override StatementContext StatementContext {
            get { return StatementContext.Select; }
            set { }
        }
        [DefaultValue(null)]
        public override object FilterValue { get; set; }

        
        [DefaultValue("City")]
        public override string FilterMemberName { get; set; }
        [DefaultValue(SizeAttribute.DefaultStringMappingFieldSize)]
        public override int FilterMemberSize { get; set; }
        [DefaultValue(true)]
        public override bool FilterMemberIndexed { get; set; }

        public override bool UseFilterValueWhenNull { get; set; }
        [DefaultValue(typeof(string))]
        public override Type FilterMemberType { get; set; }
    }
}
