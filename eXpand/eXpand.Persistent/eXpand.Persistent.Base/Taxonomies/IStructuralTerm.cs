using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using DevExpress.Xpo;

namespace eXpand.Persistent.Base.Taxonomies{
    public interface IStructuralTerm : IBaseTerm {
        Type TypeOfObject { get; set; }
        string SavedType { get; set; }
        IList<IValueTerm> DerivedTerms { get; }
        void UpdateTypes(Type[] types);
    }
}