namespace Xpand.VSIX.ModelEditor {
    public class ProjectItemWrapper {
        public bool IsApplicationProject { get; set; }

        public string Name { get; set; }

        public string OutputPath { get; set; }
        public string OutputFileName { get; set; }
        public string FullPath { get; set; }

        public string UniqueName { get; set; }

        public string LocalPath { get; set; }
        public string ModelFileName { get; set; }
    }
}