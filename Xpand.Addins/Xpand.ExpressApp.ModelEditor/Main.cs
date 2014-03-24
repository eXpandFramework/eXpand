using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using DevExpress.Persistent.Base;


namespace Xpand.ExpressApp.ModelEditor {
    public class MainClass {
        private static ModelEditorForm _modelEditorForm;
        static private void HandleException(Exception e) {
            Tracing.Tracer.LogError(e);
            Messaging.GetMessaging(null).Show(ModelEditorForm.Title, e);
        }
        static private void OnException(object sender, ThreadExceptionEventArgs e) {
            HandleException(e.Exception);
        }
        static void CheckAssemblyFile(PathInfo pathInfo) {
            if (!File.Exists(pathInfo.AssemblyPath)) {
                pathInfo.AssemblyPath = Path.Combine(Environment.CurrentDirectory, pathInfo.AssemblyPath);
                if (!File.Exists(pathInfo.AssemblyPath)) {
                    throw new Exception(String.Format("The file '{0}' couldn't be found.", pathInfo.AssemblyPath));
                }
            }
        }

        [STAThread]
        public static void Main(string[] args) {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += OnException;
            try {
                var appSetting = ConfigurationManager.AppSettings["debug"];
                if (appSetting != null && appSetting.ToLower() == "true"){
                    MessageBox.Show("Attach to this proccess");
                }
                var pathInfo = new PathInfo(args);
                Tracing.Tracer.LogSeparator("PathInfo");
                Tracing.Tracer.LogText(pathInfo.ToString());
                Tracing.Tracer.LogSeparator("PathInfo");
                CheckAssemblyFile(pathInfo);
                var modelControllerBuilder = new ModelControllerBuilder();
                var settingsStorageOnRegistry = new SettingsStorageOnRegistry(@"Software\Developer Express\eXpressApp Framework\Model Editor");
                _modelEditorForm = new ModelEditorForm(modelControllerBuilder.GetController(pathInfo), settingsStorageOnRegistry);
                _modelEditorForm.Disposed += (sender, eventArgs) => ((IModelEditorSettings)_modelEditorForm).ModelEditorSaveSettings();
                _modelEditorForm.SetCaption(Path.GetFileName(pathInfo.LocalPath));

                Application.Run(_modelEditorForm);
            } catch (Exception exception) {
                HandleException(exception);
            }

        }


    }
}
