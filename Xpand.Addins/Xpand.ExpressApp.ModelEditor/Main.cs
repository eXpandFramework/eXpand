using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelDifference;
using System.Linq;


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
		        ApplicationModulesManager applicationModulesManager = GetApplicationModulesManager(pathInfo);
		        ModelApplicationBase modelApplication = GetModelApplication(applicationModulesManager, pathInfo);
		        ModelEditorViewController controller = GetController(pathInfo, applicationModulesManager, modelApplication);
		        modelEditorForm = new ModelEditorForm(controller,new SettingsStorageOnRegistry(@"Software\Developer Express\eXpressApp Framework\Model Editor"));
		        modelEditorForm.Disposed +=(sender, eventArgs) => ((IModelEditorSettings) modelEditorForm).ModelEditorSaveSettings();
		        modelEditorForm.SetCaption(Path.GetFileName(pathInfo.LocalPath));

		        Application.Run(modelEditorForm);
		    }
		    catch (Exception exception) {
		        HandleException(exception);
		    }
			
		}

	    static ModelEditorViewController GetController(PathInfo pathInfo, ApplicationModulesManager applicationModulesManager, ModelApplicationBase modelApplication) {
	        var storePath = Path.GetDirectoryName(pathInfo.LocalPath);
	        var fileModelStore = new FileModelStore(storePath, Path.GetFileNameWithoutExtension(pathInfo.LocalPath));
	        var unusableStore = new FileModelStore(storePath, ModelStoreBase.UnusableDiffDefaultName);
	        return new ModelEditorViewController((IModelApplication)modelApplication, fileModelStore, unusableStore,applicationModulesManager.Modules);
	    }

        static ModelApplicationBase GetModelApplication(ApplicationModulesManager applicationModulesManager, PathInfo pathInfo){
	        var modelManager = new ApplicationModelsManager(applicationModulesManager.Modules, applicationModulesManager.ControllersManager, applicationModulesManager.DomainComponents);
            var modelApplication = (ModelApplicationBase) modelManager.CreateModelApplication();
            AddLayers(modelApplication, applicationModulesManager, pathInfo);
	        return modelApplication;
	    }

	    static ApplicationModulesManager GetApplicationModulesManager(PathInfo pathInfo) {
	        var designerModelFactory = new DesignerModelFactory();
	        ApplicationModulesManager applicationModulesManager = designerModelFactory.CreateModelManager(pathInfo.AssemblyPath, string.Empty);
	        applicationModulesManager.Load();
	        return applicationModulesManager;
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
            AddLayersCore(dictionary.Where(pair => PredicateLastLayer(pair, pathInfo)), modelApplication);
            AddLayersCore(dictionary.Where(pair => !PredicateLastLayer(pair, pathInfo)), modelApplication);
	    }

	    static bool PredicateLastLayer(KeyValuePair<string, ResourceInfo> pair, PathInfo pathInfo) {
            return !(pair.Key.Substring(pair.Key.LastIndexOf(".") + 1).EndsWith(Path.GetFileNameWithoutExtension(pathInfo.LocalPath) + "")) && Path.GetFileNameWithoutExtension(pathInfo.AssemblyPath)==pair.Value.AssemblyName;
	    }

	    static void AddLayersCore(IEnumerable<KeyValuePair<string, ResourceInfo>> layers, ModelApplicationBase modelApplication) {
	        foreach (var pair in layers){
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
