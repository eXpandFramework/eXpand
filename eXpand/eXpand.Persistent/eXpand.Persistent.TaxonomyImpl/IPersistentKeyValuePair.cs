using System.Xml.Serialization;

namespace eXpand.Persistent.TaxonomyImpl{
    public interface IPersistentKeyValuePair {
        [XmlAttribute]
        string Key { get; set; }

        [XmlAttribute]
        string Value { get; set; }

        [XmlAttribute]
        BaseObjectInfoValidity Validity { get; set; }
    }
}