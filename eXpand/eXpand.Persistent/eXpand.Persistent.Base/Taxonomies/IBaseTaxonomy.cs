using System.Collections.Generic;

namespace eXpand.Persistent.Base.Taxonomies{
    public interface IBaseTaxonomy {
        string Key { get; set; }
        string Name { get; set; }
        string Group { get; set; }
        string Description { get; set; }

        IList<IValueTerm> Terms { get; }
        IList<IStructuralTerm> Structure { get; }
    }
}