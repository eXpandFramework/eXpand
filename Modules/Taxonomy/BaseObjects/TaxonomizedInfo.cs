using System;
using System.Xml.Serialization;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using eXpand.Persistent.TaxonomyImpl;

namespace eXpand.ExpressApp.Taxonomy.BaseObjects{
    [Serializable]
    public class TaxonomizedInfo : BasicInfo, ICategorizedItem{
        private Term term;
        public TaxonomizedInfo() {}

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
        ITreeNode ICategorizedItem.Category{
            get { return term; }
            set { term = (Term) value; }
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

        public override void AfterConstruction(){
            Term = InfosTaxonomy.GetInstance(Session).FindInfoTerm(this.ClassInfo.ClassType);
        }
    }
}