using System.Linq;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.ImportWizard.Core {
    [Persistent]
    public class ImportMap : XPObject {
        [NonPersistent]
        public string Columns {
            get {
                const string delimeter = ";";
                return Mappings.Count > 0 ?
                    Mappings
                    .OrderBy(p => p.Column)
                    .Select(p => p.Column)
                    .Aggregate((i, j) => i + delimeter + j) :
                    string.Empty
                    ;
            }
        }


        [Association("Sheet-Mappings")]
        [DisplayName("Mappings")]
        [Aggregated]
        public XPCollection<Mapping> Mappings {
            get { return GetCollection<Mapping>("Mappings"); }
        }

        private string _Description;

        [DisplayName("Description"), Size(SizeAttribute.Unlimited)]
        public string Description {
            get { return _Description; }
            set { SetPropertyValue("Description", ref _Description, value); }
        }



        public override string ToString() {
            return string.Format("{0} - {1}", Description, Columns);
        }

        public ImportMap(Session session)
            : base(session) {
        }

        public ImportMap() {
        }
    }
}