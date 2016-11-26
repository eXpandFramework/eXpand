
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using DevExpress.Persistent.Base;


namespace Xpand.ExpressApp.ModelEditor {
    public class MainClass {
        private static ModelEditorForm _modelEditorForm;
        private static void HandleException(Exception e) {
            Tracing.Tracer.LogError(e);
            Messaging.GetMessaging(null).Show(ModelEditorForm.Title, e);
        }
        private static void OnException(object sender, ThreadExceptionEventArgs e) {
            HandleException(e.Exception);
        }
        static void CheckAssemblyFile(PathInfo pathInfo) {
            if (!File.Exists(pathInfo.AssemblyPath)) {
                pathInfo.AssemblyPath = Path.Combine(Environment.CurrentDirectory, pathInfo.AssemblyPath);
                if (!File.Exists(pathInfo.AssemblyPath)) {
                    throw new Exception($"The file '{pathInfo.AssemblyPath}' couldn't be found.");
                }
            }
        }

        [STAThread]
        public static void Main(string[] args) {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += OnException;
            try {
                var strings = args;
                if (args.Length>4&&args[0]=="d"){
                    MessageBox.Show("Attach to this proccess");
                    strings = args.Skip(1).ToArray();
                }
                var pathInfo = new PathInfo(strings);
                Tracing.Tracer.LogSeparator("PathInfo");
                Tracing.Tracer.LogText(pathInfo.ToString());
                Tracing.Tracer.LogSeparator("PathInfo");
                CheckAssemblyFile(pathInfo);
                var modelControllerBuilder = new ModelControllerBuilder();
                var settingsStorageOnRegistry = new SettingsStorageOnRegistry(@"Software\Developer Express\eXpressApp Framework\Model Editor");
                var modelEditorViewController = modelControllerBuilder.GetController(pathInfo);
                Tracing.Tracer.LogText("modelEditorViewController");
                _modelEditorForm = new ModelEditorForm(modelEditorViewController, settingsStorageOnRegistry);
                _modelEditorForm.Disposed += (sender, eventArgs) => ((IModelEditorSettings)_modelEditorForm).ModelEditorSaveSettings();
                _modelEditorForm.SetCaption(Path.GetFileName(pathInfo.LocalPath));

                Application.Run(_modelEditorForm);
            } catch (Exception exception) {
                HandleException(exception);
            }

        }


    }
}
