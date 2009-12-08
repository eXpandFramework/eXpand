using System;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.FilterDataStore.Core;

namespace eXpand.ExpressApp.FilterDataStore.Providers
{
    public class UserFilterProvider:FilterProviderBase
    {
        public override object FilterValue
        {
            get {
                if (SecuritySystem.CurrentUser != null)
                    return ((XPBaseObject)SecuritySystem.CurrentUser).ClassInfo.KeyProperty.GetValue(SecuritySystem.CurrentUser);
                return null;
            }
        }

        public override string FilterMemberName
        {
            get { return "User"; }
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
            get { return typeof(Guid); }
        }
    }
}