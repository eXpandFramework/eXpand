using System;
using DevExpress.LookAndFeel;
using DevExpress.Xpo;
using eXpand.ExpressApp.FilterDataStore.Core;

namespace eXpand.ExpressApp.FilterDataStore.Win.Providers
{
    public class SkinFilterProvider:FilterProviderBase
    {
        public override object FilterValue
        {
            get { return UserLookAndFeel.Default.ActiveSkinName; }
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
    }
}