using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SystemTester.Module.Win.FunctionalTests.FolderBrowseEditor{
    [DefaultClassOptions]
    public class FolderBrowseEditorObject : BaseObject{
        private string _path;

        public FolderBrowseEditorObject(Session session) : base(session){
        }

        public string Path{
            get { return _path; }
            set { SetPropertyValue("Path", ref _path, value); }
        }
    }
}