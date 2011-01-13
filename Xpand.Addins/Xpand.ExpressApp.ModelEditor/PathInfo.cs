using System;

namespace Xpand.ExpressApp.ModelEditor {
    public class PathInfo {
        readonly string _fullPath;
        readonly string _localPath;

        public PathInfo(string[] args) {
            AssemblyPath = args[0].TrimStart(Convert.ToChar("\"")).TrimEnd(Convert.ToChar("\""));
            _fullPath = args[2].TrimStart(Convert.ToChar("\"")).TrimEnd(Convert.ToChar("\""));
            _localPath = args[1].TrimStart(Convert.ToChar("\"")).TrimEnd(Convert.ToChar("\""));

        }

        public override string ToString() {
            return string.Format("AssemblyPath={0}{1}FullPath={2}{3}LocalPath={4}", AssemblyPath, Environment.NewLine, FullPath, Environment.NewLine, LocalPath);
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