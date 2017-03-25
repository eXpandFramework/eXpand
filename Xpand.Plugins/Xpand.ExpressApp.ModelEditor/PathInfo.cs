using System;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.ModelEditor {
    public class PathInfo {
        public PathInfo(string[] args) {
            Tracing.Tracer.LogValue("PathInfo", args);
            AssemblyPath = args[1].TrimStart(Convert.ToChar("\"")).TrimEnd(Convert.ToChar("\""));
            FullPath = args[3].TrimStart(Convert.ToChar("\"")).TrimEnd(Convert.ToChar("\""));
            LocalPath = args[2].TrimStart(Convert.ToChar("\"")).TrimEnd(Convert.ToChar("\""));
            IsApplicationModel = Convert.ToBoolean(args[0].Trim());
        }

        public bool IsApplicationModel { get;  }

        public override string ToString() {
            return
                $"AssemblyPath={AssemblyPath}{Environment.NewLine}FullPath={FullPath}{Environment.NewLine}LocalPath={LocalPath}";
        }

        public string AssemblyPath { get; set; }

        public string FullPath { get; }

        public string LocalPath { get; }
    }
}