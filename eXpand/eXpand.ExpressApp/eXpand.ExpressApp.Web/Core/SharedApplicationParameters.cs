//using System;
//using System.Collections.Generic;
//using DevExpress.ExpressApp;
//using DevExpress.ExpressApp.Localization;
//using DevExpress.ExpressApp.Security;
//using DevExpress.ExpressApp.Utils;
//using DevExpress.ExpressApp.Web;
//
//namespace eXpand.ExpressApp.Web.Core{
//    public class SharedApplicationParameters<TApplicationType> : ISharedApplicationParameters where TApplicationType : WebApplication, new() {
//        protected static readonly object LockObject = new object();
//        private ExpressApplicationSetupParameters _parameters;
//        private TApplicationType _sharedApplication;
//        
//        WebApplication ISharedApplicationParameters.SharedApplication {
//            get { return _sharedApplication; }
//        }
//
//        public TApplicationType SharedApplication{
//            get { return _sharedApplication; }
//        }
//
//        public ExpressApplicationSetupParameters Parameters{
//            get { return _parameters; }
//        }
//
//        private List<Type> GetAllLocalizedTypes(ModuleList parametersModules){
//            List<Type> allLocalizedTypes = _sharedApplication.ResourcesExportedToModel != null 
//                                               ? new List<Type>(Enumerator.Combine(_sharedApplication.ResourcesExportedToModel, ExceptionLocalizer.ExceptionLocalizers)) 
//                                               : new List<Type>(ExceptionLocalizer.ExceptionLocalizers);
//
//            foreach (ModuleBase module in parametersModules){
//                foreach (Type currentlocalizerType in module.ResourcesExportedToModel){
//                    if (allLocalizedTypes.IndexOf(currentlocalizerType) == -1){
//                        allLocalizedTypes.Add(currentlocalizerType);
//                    }
//                }
//            }
//            return allLocalizedTypes;
//        }
//
//        private void ActivateResourceLocalizers(ModuleList parametersModules){
//            List<Type> allLocalizedTypes = GetAllLocalizedTypes(parametersModules);
//            DictionaryNode localizationNode = _parameters.Model.RootNode.GetChildNode(CaptionHelper.localizationNodeName);
//            foreach (Type localizerType in allLocalizedTypes){
//                var localizer = Activator.CreateInstance(localizerType) as IXafResourceLocalizer;
//                if (localizer != null){
//                    localizer.Setup(_parameters.Model.RootNode);
//                    localizer.Activate();
//                }
//            }
//        }
//
//        public void CreateSharedParameters(string connectionString){
//            lock (LockObject){
//                if (_parameters == null){
//                    _sharedApplication = new TApplicationType();
//                    _sharedApplication.SettingUp +=
//                        delegate(object sender, SetupEventArgs e) { _parameters = e.SetupParameters; };
//                    _sharedApplication.ConnectionString = connectionString;
//                    _sharedApplication.Setup();
//                }
//            }
//        }
//
//        public TApplicationType CreateApplicationInstance() {
//            return CreateApplicationInstance(SharedApplication.Security);
//        }
//
//        public TApplicationType CreateApplicationInstance(ISecurity security){
//            var application = new TApplicationType();
//            ExpressApplicationSetupParameters instanceParameters = _parameters.Clone();
//            instanceParameters.Model = _parameters.Model.Clone();
//            instanceParameters.Modules = application.Modules;
//            ActivateResourceLocalizers(instanceParameters.Modules);
//            CaptionHelper.Setup(instanceParameters.Model.RootNode);
//            application.Security = security;
//            SecuritySystem.SetInstance(application.Security);
//            application.ConnectionString = _sharedApplication.ConnectionString;
//            application.Setup(instanceParameters);
//            return application;
//        }
//
//        WebApplication ISharedApplicationParameters.CreateApplicationInstance(ISecurity security){
//            return CreateApplicationInstance(security);
//        }
//    }
//}