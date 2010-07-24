using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.FilterDataStore.Core;

namespace eXpand.ExpressApp.FilterDataStore.Providers
{
    public class UserFilterProvider:FilterProviderBase
    {
        public override object FilterValue {
            get {
                if (SecuritySystem.CurrentUser != null)
                    return ((XPBaseObject)SecuritySystem.CurrentUser).ClassInfo.KeyProperty.GetValue(
                            SecuritySystem.CurrentUser);
                return UpdaterUserKey;
            }
            set { }
        }

        [DefaultValue("UserFilter")]
        public override string FilterMemberName { get; set; }
        [DefaultValue(SizeAttribute.DefaultStringMappingFieldSize)]
        public override int FilterMemberSize { get; set; }
        [DefaultValue(true)]
        public override bool FilterMemberIndexed { get; set; }
        
        public override bool UseFilterValueWhenNull { get; set; }
        [DefaultValue(typeof(Guid))]
        public override Type FilterMemberType { get; set; }

        public static object UpdaterUserKey { get; set; }
    }
}