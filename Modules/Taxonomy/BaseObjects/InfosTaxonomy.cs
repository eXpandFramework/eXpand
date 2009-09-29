using System;
using System.Linq;
using DevExpress.Xpo;

namespace eXpand.ExpressApp.Taxonomy.BaseObjects{
    [Serializable]
    public class InfosTaxonomy : Taxonomy{
        protected internal InfosTaxonomy(Session session) : base(session) {}

        public static InfosTaxonomy GetInstance(Session session){
            var result = session.FindObject<InfosTaxonomy>(null);
            if (result == null){
                result = new InfosTaxonomy(session){Key = "infos", Name = "Provided Infos", Group = "System"};
                result.Save();
            }
            return result;
        }

        public Term RegisterInfoType(Type infoType, string infoTypeGroup) {
            return AddTerm(Session, string.Format("/{0}/{1}", infoTypeGroup, infoType.Name), string.Empty);
        }

        public Term FindInfoTerm(Type infoType) {
            return Terms.Where(t => t.Key == infoType.Name).SingleOrDefault();
        }
    }
}