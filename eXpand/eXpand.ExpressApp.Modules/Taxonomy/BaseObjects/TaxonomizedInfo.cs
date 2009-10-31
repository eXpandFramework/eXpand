using System;
using System.ComponentModel;
using System.Xml.Serialization;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using eXpand.Persistent.TaxonomyImpl;

namespace eXpand.ExpressApp.Taxonomy.BaseObjects{
    [Serializable]
    [DefaultClassOptions]
    public class TaxonomizedInfo : BaseInfo, ICategorizedItem {
        private Term term;
        

        public TaxonomizedInfo(Session session) : base(session) {}

        [XmlIgnore]
        [Association(Associations.TermTaxonomizedInfos)]
        public Term Term{
            get { return term; }
            set { SetPropertyValue("Term", ref term, value); }
        }

        [XmlIgnore]
        public Term Category{
            get { return term; }
            set { term = value; }
        }
        #region ICategorizedItem Members
        [XmlIgnore]
        ITreeNode ICategorizedItem.Category {
            get { return term; }
            set { term = (Term)value; }
        }
        #endregion
        protected override void OnSaving(){
            if (string.IsNullOrEmpty(Key)){
                Key = term.Key;
            }
            if (string.IsNullOrEmpty(Value)){
                Value = term.FullPath;
            }
            base.OnSaving();
        }

        public static TaxonomizedInfo TaxonomizeObject(Term term, BaseObject objectToTaxonomize) {
            var info = new TaxonomizedInfo(term.Session);
            info.BaseObjects.Add(objectToTaxonomize);
            info.Term = term;
            return info;
        } 
    }
}