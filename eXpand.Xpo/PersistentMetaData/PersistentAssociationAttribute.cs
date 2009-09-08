using System;
using DevExpress.Xpo;

namespace eXpand.Xpo.PersistentMetaData
{
    public class PersistentAssociationAttribute : PersistentAttributeInfo {
        public PersistentAssociationAttribute(Session session) : base(session) { }

        public PersistentAssociationAttribute()
        {
        }

        string _an;
        public string AssociationName { get { return _an; } set { SetPropertyValue("AssociationName", ref _an, value); } }

        string _ean;
        public string ElementAssemblyName { get { return _ean; } set { SetPropertyValue("ElementAssemblyName", ref _ean, value); } }

        string _etn;
        public string ElementTypeName { get { return _etn; } set { SetPropertyValue("ElementTypeName", ref _etn, value); } }

        public override Attribute Create() {
            return new AssociationAttribute(AssociationName, ElementAssemblyName, ElementTypeName);
        }
    }
}