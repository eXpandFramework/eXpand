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
using eXpand.Utils.DependentAssembly;
using System.Linq;
using ResourcesModelStore = eXpand.ExpressApp.ModelDifference.Core.ResourcesModelStore;

namespace eXpand.ExpressApp.ModelEditor {
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
		static void Main(string[] args) {
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			Application.ThreadException += OnException;
		    

			if (args.Length == 0 ||args.Length > 3) {
                MessageBox.Show(string.Format("Usage: {0}'{1}' <Path_to_app_config_file> {0} '{1}' <Path_to_dll_file> <Path_to_diff_file>", 
                                                Environment.NewLine, Environment.GetCommandLineArgs()[0]), ModelEditorForm.Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else {
				try {
				    
                    var pathInfo = new PathInfo(args);
                    if (Path.GetExtension(pathInfo.AssemblyPath).ToLower() != ".config" && Path.GetExtension(pathInfo.AssemblyPath).ToLower() != ".dll")
                    {
                        throw new Exception("This file can be executed with a configuration or an assebly file as a parameter.");
                    }

                    if (!File.Exists(pathInfo.AssemblyPath))
                    {
                        pathInfo.AssemblyPath = Path.Combine(Environment.CurrentDirectory, pathInfo.AssemblyPath);
                        if (!File.Exists(pathInfo.AssemblyPath))
                        {
                            throw new Exception(String.Format("The config file '{0}' couldn't be found.", pathInfo.AssemblyPath));
						}
					}
                     
					var designerModelFactory = new DesignerModelFactory();
                    ApplicationModulesManager applicationModulesManager = designerModelFactory.CreateModelManager(pathInfo.AssemblyPath, string.Empty);
					applicationModulesManager.Load();
                    var modelManager = new ApplicationModelsManager(applicationModulesManager.Modules, applicationModulesManager.ControllersManager, applicationModulesManager.DomainComponents);
                    var fileModelStore = new FileModelStore(Path.GetDirectoryName(pathInfo.LocalPath), Path.GetFileNameWithoutExtension(pathInfo.LocalPath));

				    IModelApplication modelApplication = modelManager.CreateModelApplication(fileModelStore);                    
				    AddLayers((ModelApplicationBase) modelApplication,pathInfo.AssemblyPath,applicationModulesManager);
				    var controller = new ModelEditorViewController(modelApplication, fileModelStore, applicationModulesManager.Modules);
				    modelEditorForm = new ModelEditorForm(controller, new SettingsStorageOnRegistry(@"Software\Developer Express\eXpressApp Framework\Model Editor"));
                    modelEditorForm.Disposed += (sender, eventArgs) => ((IModelEditorSettings)modelEditorForm).ModelEditorSaveSettings();
					modelEditorForm.SetCaption(Path.GetFileName(pathInfo.LocalPath));
                    
					Application.Run(modelEditorForm);
				} catch(Exception exception) {
					HandleException(exception);
				}
			}
		}

	    static void AddLayers(ModelApplicationBase modelApplication, string fullPath, ApplicationModulesManager applicationModulesManager) {
	        var lastLayer = modelApplication.LastLayer;
            modelApplication.RemoveLayer(lastLayer);
	        IEnumerable<string> assemblyPaths = DependentAssemblyPathResolver.GetAssemblyPaths(fullPath).Reverse();
	        foreach (var assemblyPath in assemblyPaths) {
	            string path = assemblyPath;
	            var assembly = applicationModulesManager.Modules.Where(mbase => mbase.GetType().Assembly.Location==path).Select(mbase => mbase.GetType().Assembly).Single();
	            var layer = modelApplication.CreatorInstance.CreateModelApplication();
	            modelApplication.AddLayer(layer);
	            var resourcesModelStore = new ResourcesModelStore(assembly, "");
                resourcesModelStore.Load(layer);                
	        }
	        modelApplication.AddLayer(lastLayer);
	    }
	}
}
