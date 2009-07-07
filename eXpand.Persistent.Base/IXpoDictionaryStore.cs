using System;

namespace eXpand.Persistent.Base
{
    public interface IXpoDictionaryStore
    {
        DifferenceType DifferenceType { get; set; }
        string Name { get; set; }
        string ApplicationTypeName { get; set; }
        bool Disable { get; set; }
        string Aspect { get; set; }
        DateTime DateCreated { get; set; }
        string XmlContent { get; set; }
    }
}