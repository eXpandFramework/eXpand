using System;
using System.Reflection;
using System.Xml.Serialization;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using eXpand.Persistent.TaxonomyImpl;
using eXpand.Xpo;

namespace eXpand.ExpressApp.Taxonomy.BaseObjects{
    [Serializable]
    public class TermAssignment : BaseObjectInfo{ //}, ICategorizedItem {
        private Term term;
        public TermAssignment() {}

        public TermAssignment(Session session) : base(session) { }

        [XmlIgnore]
        [Association(Associations.TermTermAssignments)]
        public Term Term{
            get { return term; }
            set { SetPropertyValue("Term", ref term, value); }
        }

        public override string Key{
            get { return term.Key; }
            set { throw new NotImplementedException(); }
        }

        public override string Value{
            get { return term.FullPath; }
            set { throw new NotImplementedException(); }
        }

        [XmlIgnore]
        public Term Category{
            get { return term; }
            set { term = value; }
        }
        //#region ICategorizedItem Members
        //[XmlIgnore]
        //ITreeNode ICategorizedItem.Category{
        //    get { return term; }
        //    set { term = (Term) value; }
        //}
        //#endregion
    }
}