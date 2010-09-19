using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using DevExpress.CodeRush.Core;

namespace XpandAddIns.ModelEditor {
    public class ModelEditorRunner {
        public void Start(ProjectWrapper projectWrapper)
        {
            string outputFileName = projectWrapper.OutPutFileName;
            if (outputFileName.ToLower().EndsWith(".exe"))
                outputFileName += ".config";
            using (var storage = new DecoupledStorage(typeof(Options)))
            {
                string path = storage.ReadString(Options.GetPageName(), "modelEditorPath");
                if (!String.IsNullOrEmpty(path))
                {
                    StartMEProcess(projectWrapper, outputFileName, path);
                    return;
                }
                const string modeleditorpathPathIsEmpty = "ModelEditorPath path is empty";
                MessageBox.Show(modeleditorpathPathIsEmpty);
            }    
        }
        void StartMEProcess(ProjectWrapper projectWrapper, string outputFileName, string path)
        {
            var fullPath = projectWrapper.FullPath;
            string assemblyPath = Path.Combine(fullPath, Path.Combine(projectWrapper.OutputPath, outputFileName));
            if (!File.Exists(assemblyPath))
            {
                MessageBox.Show(String.Format(@"Assembly {0} not found", assemblyPath), null, MessageBoxButtons.OK);
                return;
            }

            string arguments = String.Format("'{0}' '{1}' '{2}'", Path.GetFullPath(assemblyPath), fullPath, projectWrapper.LocalPath);
            if (File.Exists(path))
                Process.Start(path, arguments);
            else
                MessageBox.Show(String.Format("Model editor not found at {0}", path));
        }

    }
}