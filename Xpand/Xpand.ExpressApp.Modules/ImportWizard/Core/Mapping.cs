using DevExpress.Xpo;

namespace Xpand.ExpressApp.ImportWiz.Core
{
    [Persistent]
    public class Mapping : XPObject
    {

        private ImportMap _Map;
        [Association("Sheet-Mappings")]
        [DisplayName("Map")]
        public ImportMap Map
        {
            get { return _Map; }
            set { SetPropertyValue("Map", ref _Map, value); }
        }

        private string _Column;
        [DisplayName("Column")]
        public string Column
        {
            get { return _Column; }
            set { SetPropertyValue("Column", ref _Column, value); }
        }

        private string _MapedTo;
        [DisplayName("MapedTo")]
        public string MapedTo
        {
            get { return _MapedTo; }
            set { SetPropertyValue("MapedTo", ref _MapedTo, value); }
        }


        public Mapping(Session session)
            : base(session)
        {
        }

        public Mapping()
        {
        }
    }
}
