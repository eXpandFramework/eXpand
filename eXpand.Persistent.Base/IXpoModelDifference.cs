using System;

namespace eXpand.Persistent.Base
{
    public interface IXpoModelDifference
    {
        DifferenceType DifferenceType { get; }
        string Name { get; set; }
        bool Disabled { get; set; }
        DateTime DateCreated { get; set; }
        string XmlContent { get; set; }
    }
}