using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace eXpand.Persistent.BaseImpl
{
    [DefaultProperty("Term")]
    [FriendlyKeyProperty("Term")]
    public class TermInfo : BaseObject{
        public TermInfo(Session session) : base(session) {}

        private Term term;
        [Association("Term-TermInfos"), Aggregated]
        public Term Term {
            get {
                return term;
            }
            set {
                SetPropertyValue("Term", ref term, value);
            }
        }

        private bool isCertain;
        public bool IsCertain {
            get {
                return isCertain;
            }
            set {
                SetPropertyValue("IsCertain", ref isCertain, value);
            }
        }
    }
}