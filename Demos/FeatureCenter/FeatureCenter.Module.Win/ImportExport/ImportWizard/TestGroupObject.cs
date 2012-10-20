using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace FeatureCenter.Module.Win.ImportExport
{
    
    public class TestGroupObject : BaseObject
    {
        private string _Name;

        [DisplayName("Name")]
        public string Name
        {
            get { return _Name; }
            set { SetPropertyValue("Name", ref _Name, value); }
        }


        public TestGroupObject(Session session) : base(session)
        {
        }

        public TestGroupObject()
        {
        }
    }
}
