using System;
using System.Collections.Generic;
using System.Reflection;
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
using eXpand.Utils.DependentAssembly;
using System.Linq;
using ResourcesModelStore = eXpand.Persistent.Base.ModelDifference.ResourcesModelStore;


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
            IEnumerable<string> assemblyPaths = DependentAssemblyPathResolver.GetAssemblyPaths(pathInfo.AssemblyPath).Reverse();
	        string resourceName = null;
	        foreach (var assemblyPath in assemblyPaths) {
	            var assembly = GetAssembly(applicationModulesManager, assemblyPath);
	            string addLayerCore = AddLayerCore(pathInfo, modelApplication, assembly);
                if (addLayerCore != null)
                    resourceName = addLayerCore;
	        }
	        var lastLayer = modelApplication.CreatorInstance.CreateModelApplication();
	        modelApplication.AddLayer(lastLayer);
            new ModelXmlReader().ReadFromResource(lastLayer,"",GetAssembly(applicationModulesManager, pathInfo.AssemblyPath), resourceName);
	    }

	    static string AddLayerCore(PathInfo pathInfo, ModelApplicationBase modelApplication, Assembly assembly) {
	        string resourceName = null;
	        var layer = modelApplication.CreatorInstance.CreateModelApplication();
	        modelApplication.AddLayer(layer);
	        var resourcesModelStore = new ResourcesModelStore(assembly, "", true);
	        resourcesModelStore.ResourceLoading += (sender, args) => {
	            if (args.ResourceName.EndsWith(Path.GetFileName(pathInfo.LocalPath))) {
                    args.Cancel = assembly.Location == pathInfo.AssemblyPath;
                    if (args.Cancel)
	                    resourceName = args.ResourceName;
	            }
	        };
	        resourcesModelStore.Load(layer);
            return resourceName;
	    }

	    static Assembly GetAssembly(ApplicationModulesManager applicationModulesManager, string path) {
	        return applicationModulesManager.Modules.Where(mbase => mbase.GetType().Assembly.Location==path).Select(mbase => mbase.GetType().Assembly).Single();
	    }
	}
}
