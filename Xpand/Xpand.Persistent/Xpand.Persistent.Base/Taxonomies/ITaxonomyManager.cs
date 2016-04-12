using System;
using DevExpress.Xpo;

namespace eXpand.Persistent.Base.Taxonomies {
    public interface ITaxonomyManager {     
        string[] GetPathSegments(string termPath);
        string StructurePath(string termPath);
    }
}