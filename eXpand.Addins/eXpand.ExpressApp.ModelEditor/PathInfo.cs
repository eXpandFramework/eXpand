using System;

namespace eXpand.ExpressApp.ModelEditor {
    public class PathInfo {
        readonly string _fullPath;
        readonly string _localPath;

        public PathInfo(string[] args) {
            AssemblyPath = args[0].TrimStart(Convert.ToChar("'")).TrimEnd(Convert.ToChar("'"));
            _fullPath = args[1].TrimStart(Convert.ToChar("'")).TrimEnd(Convert.ToChar("'"));
            _localPath = args[2].TrimStart(Convert.ToChar("'")).TrimEnd(Convert.ToChar("'"));

        }


        public string AssemblyPath { get; set; }

        public string FullPath {
            get { return _fullPath; }
        }

        public string LocalPath {
            get { return _localPath; }
        }
    }
}