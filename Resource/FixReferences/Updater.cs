namespace FixReferences {
    abstract class Updater {
        readonly IDocumentHelper _documentHelper;
        readonly string _rootDir;

        protected Updater(IDocumentHelper documentHelper,string rootDir) {
            _documentHelper = documentHelper;
            _rootDir = rootDir;
        }

        public IDocumentHelper DocumentHelper {
            get { return _documentHelper; }
        }

        public string RootDir {
            get { return _rootDir; }
        }

        public abstract void Update(string file);

    }
}