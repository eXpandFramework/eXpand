using DevExpress.Xpo;

namespace Xpand.ExpressApp.ImportWizard.Core{
    [Persistent]
    public class Mapping : XPObject{
        private string _column;
        private ImportMap _map;

        private string _mapedTo;


        public Mapping(Session session)
            : base(session){
        }

        public Mapping(){
        }

        [Association(@"Sheet-Mappings")]
        [DisplayName(@"Map")]
        public ImportMap Map{
            get { return _map; }
            set { SetPropertyValue(@"Map", ref _map, value); }
        }

        [DisplayName(@"Column")]
        public string Column{
            get { return _column; }
            set { SetPropertyValue(@"Column", ref _column, value); }
        }

        [DisplayName(@"MapedTo")]
        public string MapedTo{
            get { return _mapedTo; }
            set { SetPropertyValue(@"MapedTo", ref _mapedTo, value); }
        }
    }
}