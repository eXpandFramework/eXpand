using System;
using System.ComponentModel;
using DevExpress.Xpo;
using eXpand.ExpressApp.FilterDataStore.Core;

namespace eXpand.ExpressApp.FilterDataStore.Win.Providers
{
    public sealed class SkinFilterProvider:FilterProviderBase
    {
        public override object FilterValue { get; set; }

        [DefaultValue("Skin")]
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