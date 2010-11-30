using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.ModelDifference;


namespace Xpand.ExpressApp.ModelEditor {
	public class MainClass {
		private static ModelEditorForm modelEditorForm;
		static private void HandleException(Exception e) {
			Tracing.Tracer.LogError(e);
			Messaging.GetMessaging(null).Show(ModelEditorForm.Title, e);
		}
		static private void OnException(object sender, ThreadExceptionEventArgs e) {
			HandleException(e.Exception);
		}
		[STAThread]
		public static void Main(string[] args) {
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			Application.ThreadException += OnException;

		    try {
            
		        var pathInfo = new PathInfo(args);
		        Tracing.Tracer.LogSeparator("PathInfo");
		        Tracing.Tracer.LogText(pathInfo.ToString());
                Tracing.Tracer.LogSeparator("PathInfo");
		        CheckAssemblyFile(pathInfo);
                DesignerModelFactory dmf = new DesignerModelFactory();
                string assemblyPath = Path.GetDirectoryName(pathInfo.AssemblyPath);
                FileModelStore fileModelStore = dmf.CreateModuleModelStore(Path.GetDirectoryName(pathInfo.LocalPath));
                var module = dmf.CreateModuleFromFile(pathInfo.AssemblyPath, assemblyPath);
                ApplicationModulesManager applicationModulesManager = dmf.CreateModulesManager(module, assemblyPath);
		        ModelApplicationBase modelApplication = GetModelApplication(dmf, module, applicationModulesManager, fileModelStore, pathInfo);
                ModelEditorViewController controller = new ModelEditorViewController((IModelApplication)modelApplication, fileModelStore);
		        modelEditorForm = new ModelEditorForm(controller,new SettingsStorageOnRegistry(@"Software\Developer Express\eXpressApp Framework\Model Editor"));
		        modelEditorForm.Disposed +=(sender, eventArgs) => ((IModelEditorSettings) modelEditorForm).ModelEditorSaveSettings();
		        modelEditorForm.SetCaption(Path.GetFileName(pathInfo.LocalPath));

		        Application.Run(modelEditorForm);
		    }
		    catch (Exception exception) {
		        HandleException(exception);
		    }
			
		}

        static ModelApplicationBase GetModelApplication(DesignerModelFactory dmf, ModuleBase module, ApplicationModulesManager modulesManager, FileModelStore fileModelStore, PathInfo pathInfo){
            var modelApplication =  (ModelApplicationBase)dmf.CreateApplicationModel(module, modulesManager, fileModelStore);
            ////AddLayers(modelApplication, modulesManager, pathInfo);
	        return modelApplication;
	    }

	    static void CheckAssemblyFile(PathInfo pathInfo) {
	        if (!File.Exists(pathInfo.AssemblyPath)){
	            pathInfo.AssemblyPath = Path.Combine(Environment.CurrentDirectory, pathInfo.AssemblyPath);
	            if (!File.Exists(pathInfo.AssemblyPath)){
	                throw new Exception(String.Format("The file '{0}' couldn't be found.", pathInfo.AssemblyPath));
	            }
	        }
	    }

	    static void AddLayers(ModelApplicationBase modelApplication, ApplicationModulesManager applicationModulesManager, PathInfo pathInfo) {
	        var resourceModelCollector = new ResourceModelCollector();
	        var dictionary = resourceModelCollector.Collect(applicationModulesManager.Modules.Select(@base => @base.GetType().Assembly), null);
            AddLayersCore(dictionary.Where(pair => !PredicateLastLayer(pair, pathInfo)), modelApplication);
            ModelApplicationBase lastLayer = modelApplication.CreatorInstance.CreateModelApplication();
	        modelApplication.AddLayer(lastLayer);
	    }

	    static bool PredicateLastLayer(KeyValuePair<string, ResourceInfo> pair, PathInfo pathInfo) {
            var name =pair.Key.EndsWith(ModelStoreBase.ModelDiffDefaultName)?ModelStoreBase.ModelDiffDefaultName: pair.Key.Substring(pair.Key.LastIndexOf(".") + 1);
	        bool nameMatch = (name.EndsWith(Path.GetFileNameWithoutExtension(pathInfo.LocalPath) + ""));
	        bool assemblyMatch = Path.GetFileNameWithoutExtension(pathInfo.AssemblyPath)==pair.Value.AssemblyName;
	        return nameMatch && assemblyMatch;
	    }

	    static void AddLayersCore(IEnumerable<KeyValuePair<string, ResourceInfo>> layers, ModelApplicationBase modelApplication) {
	        IEnumerable<KeyValuePair<string, ResourceInfo>> keyValuePairs = layers;
	        foreach (var pair in keyValuePairs){
	            ModelApplicationBase layer = modelApplication.CreatorInstance.CreateModelApplication();
	            layer.Id = pair.Key;
	            modelApplication.AddLayer(layer);
	            var modelXmlReader = new ModelXmlReader();
	            foreach (var aspectInfo in pair.Value.AspectInfos) {
	                modelXmlReader.ReadFromString(layer,aspectInfo.AspectName, aspectInfo.Xml);
	            }
	        }
	    }
	}
}
