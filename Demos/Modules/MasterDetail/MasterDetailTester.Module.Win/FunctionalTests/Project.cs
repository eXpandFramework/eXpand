using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace MasterDetailTester.Module.Win.FunctionalTests{
    [DefaultClassOptions]
    public class Project : BaseObject,ISupportMasterDetail{
        public Project(Session session) : base(session){
        }

        public string Name { get; set; }

        [Association("Project-Contributors"), AggregatedAttribute]
        public XPCollection<Contributor> Contributors{
            get { return GetCollection<Contributor>("Contributors"); }
        }
    }

    public interface ISupportMasterDetail{
    }

    public class Contributor : BaseObject, ISupportMasterDetail {
        private Project _project;

        public Contributor(Session session) : base(session){
        }

        public string Name { get; set; }

        [Association("Project-Contributors")]
        public Project Project{
            get { return _project; }
            set { SetPropertyValue("Project", ref _project, value); }
        }
    }
}