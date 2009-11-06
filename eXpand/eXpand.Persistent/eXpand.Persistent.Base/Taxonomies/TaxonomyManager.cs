using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.Xpo;

namespace eXpand.Persistent.Base.Taxonomies{
    public sealed class TaxonomyManager : ITaxonomyManager {
        private readonly Session _session;
        private readonly BaseTaxonomy _taxonomy;
        
        public TaxonomyManager(BaseTaxonomy taxonomy) : this(taxonomy.Session, taxonomy) {}
        public TaxonomyManager(Session session, BaseTaxonomy taxonomy){
            _session = session;
            _taxonomy = taxonomy;
        }

        public BaseTaxonomy GetTaxonomy<TTaxonomy>(string taxonomyKey) where TTaxonomy: BaseTaxonomy {
            return GetTaxonomy<TTaxonomy>(_session, taxonomyKey);
        }

        public TTerm GetTerm<TTerm>(string termPath) where TTerm : BaseTerm {
            return GetTerm<TTerm>(_session, termPath);
        }

        public TTerm AddTerm<TTerm>(string termPath, string termName) where TTerm : BaseTerm {
            return AddTerm<TTerm>(_taxonomy, _session, termName, termName);
        }

        public TTerm AddStructuralTerm<TTerm>(string termPath, string termName, Type[] types) where TTerm : BaseTerm, IStructuralTerm {
            return AddStructuralTerm<TTerm>(_taxonomy, _session, termPath, termName, types);
        }

        public TTerm AddValueTerm<TTerm>(string termPath, string termName) where TTerm : BaseTerm, IValueTerm {
            return AddValueTerm<TTerm>(_taxonomy, _session, termPath, termName);
        }

        public string[] GetPathSegments(string termPath) {
            return GetPathSegments(_taxonomy.Key, termPath);
        }

        public string StructurePath(string termPath) {
            return StructurePath(_taxonomy.Key, termPath);
        }
        #region Static Implementation

        public static TTaxonomy GetTaxonomy<TTaxonomy>(Session session, string taxonomyKey) where TTaxonomy : BaseTaxonomy {
            return session.FindObject<TTaxonomy>(PersistentCriteriaEvaluationBehavior.InTransaction, x => x.Key == taxonomyKey);
        }

        public static TTerm GetTerm<TTerm>(Session session, string termPath) where TTerm : BaseTerm {
            return session.FindObject<TTerm>(PersistentCriteriaEvaluationBehavior.InTransaction, t => t.FullPath == termPath);
        }

        public static TTerm AddTerm<TTerm>(BaseTaxonomy taxonomy, Session session, string termPath, string termName) where TTerm : BaseTerm {
            var term = GetTerm<TTerm>(session, StructurePath(taxonomy.Key, termPath));
            
            if (term == null) {
                string[] pathSegments = GetPathSegments(taxonomy.Key, termPath);

                if (pathSegments.Length == 0) {
                    throw new UserFriendlyException(new ArgumentException(String.Format("Invalid path: {0}", termPath)));
                }

                term = (TTerm)Activator.CreateInstance(typeof(TTerm), session);
                term.Key = pathSegments.Last();
                term.Name = String.IsNullOrEmpty(termName) ? pathSegments.Last() : termName;
                string parentPath = termPath.Substring(0, termPath.LastIndexOf("/"));
                if (!string.IsNullOrEmpty(parentPath) && parentPath != taxonomy.Key) {
                    term.ParentTerm = AddTerm<TTerm>(taxonomy, session, parentPath, String.Empty);
                } else if (session != taxonomy.Session) {
                    term.BaseTaxonomy = (IBaseTaxonomy) session.GetObject(taxonomy);
                } else{
                    term.BaseTaxonomy = taxonomy;
                }
                term.EvaluateTermPropertyValues(false);
            }
            return term;
        }

        public static TTerm AddValueTerm<TTerm>(BaseTaxonomy taxonomy, Session session, string termPath, string termName) where TTerm : BaseTerm, IValueTerm {
            return AddTerm<TTerm>(taxonomy, session, termPath, termName);
        }

        public static TTerm AddStructuralTerm<TTerm>(BaseTaxonomy taxonomy, Session session, string termPath, string termName, Type[] types) where TTerm : BaseTerm, IStructuralTerm  {
            var term = AddTerm<TTerm>(taxonomy, session, termPath, termName);
            term.UpdateTypes(types);
            return term;
        }
        
        public static string StructurePath(string taxonomyKey, string termPath) {
            return String.Format("{0}{1}"
                                 , taxonomyKey
                                 , !termPath.StartsWith("/") && !termPath.StartsWith(taxonomyKey)
                                        ? String.Format("/{0}", termPath)
                                       : termPath);
        }
        public static string[] GetPathSegments(string taxonomyKey, string termPath) {
            return termPath.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
        }
        #endregion
    }
}