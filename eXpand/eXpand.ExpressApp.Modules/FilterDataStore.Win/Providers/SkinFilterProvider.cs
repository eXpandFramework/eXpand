using System;
using DevExpress.Xpo;
using eXpand.ExpressApp.FilterDataStore.Core;

namespace eXpand.ExpressApp.FilterDataStore.Win.Providers
{
    public class SkinFilterProvider:FilterProviderBase
    {
        public override object FilterValue
        {
            get { return Skin; }
        }

        public override string FilterMemberName
        {
            get { return "Skin"; }
        }

        public override int FilterMemberSize
        {
            get { return SizeAttribute.DefaultStringMappingFieldSize; }
        }

        public override bool FilterMemberIndexed
        {
            get { return true; }
        }

        public override bool UseFilterValueWhenNull
        {
            get { return false; }
        }

        public override Type FilterMemberType
        {
            get { return typeof(string); }
        }

        public static string Skin { get; set; }
    }
}