using System;
using System.Xml.Serialization;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Xpo;

namespace eXpand.Persistent.TaxonomyImpl{
    [Serializable]
    public class PersistentKeyValuePair : eXpandLiteObject {

        public PersistentKeyValuePair(Session session) : base(session) {}
        public PersistentKeyValuePair() {}
        
        
    }
}