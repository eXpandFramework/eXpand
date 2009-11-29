using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.Attributes;
using AggregatedAttribute=DevExpress.ExpressApp.DC.AggregatedAttribute;

namespace eXpand.ExpressApp.SystemModule{
    public class ProvidedAssociationProcessInfo {
        public ProvidedAssociationAttribute ProvidedAccociationAttribute { get; set; }
        public AssociationAttribute AssociationAttribute { get; set; }
        public AggregatedAttribute AggregatedAttribute { get; set; }
        public XPMemberInfo ProviderMember { get; set; }

        public XPClassInfo ProviderClass { get; set; }

        public XPDictionary Dictionary { get; set; }

        public Type ProvidedPropertyType { get; set; }
        public ProvidedMemberAttribute ProvidedMemberAttribute { get; set; }
        
    }
}