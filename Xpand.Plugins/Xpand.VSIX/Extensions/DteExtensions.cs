using System;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSLangProj;
using Constants = EnvDTE.Constants;

namespace Xpand.VSIX.Extensions {
    public interface IDTE2Provider{
        
    }
    public static class DteExtensions {
        private static OutputWindowPane _outputWindowPane;

        public static DTE2 DTE = (DTE2) Package.GetGlobalService(typeof(DTE));

        public static Solution Solution(this IDTE2Provider provider){
            return provider.DTE2().Solution;
        }

        public static DTE2 DTE2(this IDTE2Provider provider){
            return DTE;
        }

        public static IFullReference[] GetReferences(this Solution solution) {
            return solution.Projects().Where(project => project.Object!=null).SelectMany(project => ((VSProject)project.Object).References.OfType<IFullReference>()).Where(reference =>
                reference.SpecificVersion && (reference.Identity.StartsWith("Xpand") || reference.Identity.StartsWith("DevExpress"))).ToArray();
        }

        public static void InitOutputCalls(this DTE2 dte, string text) {
            dte.WriteToOutput(Environment.NewLine + "------------------" + text + "------------------" + Environment.NewLine);
        }

        public static void LogError(this DTE2 dte, string text){
            var log = ((IServiceProvider) VSPackage.VSPackage.Instance).GetService(typeof(SVsActivityLog)) as IVsActivityLog;
            log?.LogEntry((uint) __ACTIVITYLOG_ENTRYTYPE.ALE_ERROR, VSPackage.VSPackage.Instance.ToString(), text);
        }

        public static void WriteToOutput(this DTE2 dte, string text) {
            InitializeVsPaneIfNeeded(dte);
            if (_outputWindowPane == null || string.IsNullOrEmpty(text))
                return;
            ActivatePane();
            _outputWindowPane.OutputString(text + Environment.NewLine);
        }

        private static void ActivatePane() {
            DTE.Windows.Item(Constants.vsWindowKindOutput).Activate();
            _outputWindowPane.Activate();
        }

        public static void ClearOutputWindow(this DTE2 dte) {
            InitializeVsPaneIfNeeded(dte);
            _outputWindowPane?.Clear();
        }

        static void InitializeVsPaneIfNeeded(DTE2 dte) {
            if (_outputWindowPane != null) {
                ActivatePane();
                return;
            }
            var wnd = dte.Windows.Item(Constants.vsWindowKindOutput);
            var outputWindow = wnd?.Object as OutputWindow;
            if (outputWindow == null)
                return;
            try {
                _outputWindowPane = outputWindow.OutputWindowPanes.Item("Xpand");
            }
            catch {
                _outputWindowPane = outputWindow.OutputWindowPanes.Add("Xpand");
            }
        }

    }
}
