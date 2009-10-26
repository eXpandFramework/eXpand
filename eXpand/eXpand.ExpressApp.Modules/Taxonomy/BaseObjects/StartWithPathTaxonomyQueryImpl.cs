using DevExpress.Xpo;

namespace eXpand.ExpressApp.Taxonomy.BaseObjects{
    [NonPersistent]
    public class StartWithPathTaxonomyQueryImpl : TaxonomyQuery {
        public StartWithPathTaxonomyQueryImpl() {
            CriteriaOnTerms = "[FullPath] like '{0}{PathSeparator}%'"; 
        }
    }
}