using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.Core {
    public class XpandControllersManager : ControllersManager {
        public new ReadOnlyCollection<Controller> Controllers {
            get { return base.Controllers; }
        }

    }
    public class XpandApplicationModulesManager : ApplicationModulesManager {


        private XpandControllersManager _xpandControllersManager;
        //        private ModuleList _modules;
        private readonly ISecurity security;
        private bool isLoaded;
        private string _assembliesPath;

        private readonly List<Assembly> scannedForModuleAssemblies = new List<Assembly>();
        private static readonly object lockObject = new object();
        private readonly Dictionary<Assembly, Assembly> assemblyHash = new Dictionary<Assembly, Assembly>();
        //        public new ModuleList Modules
        //        {
        //            get { return _modules; }
        //            set { _modules = value; }
        //        }

        public new XpandControllersManager ControllersManager {
            get { return _xpandControllersManager; }
        }

        private void Init(XpandControllersManager xpandControllersManager, string assembliesPath) {
            Tracing.Tracer.LogVerboseText("ApplicationModulesManager.Init");
            _assembliesPath = assembliesPath;
            Modules = new ModuleList(null);
            _xpandControllersManager = xpandControllersManager;
        }

        private void SetupModules() {
            Tracing.Tracer.LogText("SetupModules");
            foreach (ModuleBase module in Modules) {
                try {
                    ((ISupportSetup)module).Setup(this);
                } catch (Exception e) {
                    throw new InvalidOperationException(
                        string.Format("Exception occurs while initializing the '{0}' module: {1}", module.GetType().FullName, e.Message), e);
                }
            }
            foreach (Controller controller in _xpandControllersManager.Controllers) {
                var supportSetupItem = controller as ISupportSetup;
                if (supportSetupItem != null) {
                    try {
                        supportSetupItem.Setup(this);
                    } catch (Exception e) {
                        throw new InvalidOperationException(
                            string.Format("Exception occurs while the '{0}' customizes XPDictionary: {1}", controller.GetType().FullName, e.Message), e);
                    }
                }
            }
        }
        private void DoCustomizeTypesInfo(ITypesInfo typesInfo) {
            Tracing.Tracer.LogText("Customize XPDictionary");
            lock (lockObject) {
                foreach (ModuleBase module in Modules) {
                    try {
                        module.CustomizeTypesInfo(typesInfo);
                    } catch (Exception e) {
                        throw new InvalidOperationException(
                            string.Format("Exception occurs while the '{0}' customizes XPDictionary: {1}", module.GetType().FullName, e.Message), e);
                    }
                    module.CustomizeLogics(XafTypesInfo.XpoTypeInfoSource.CustomLogics);
                }
                foreach (Controller controller in _xpandControllersManager.Controllers) {
                    try {
                        controller.CustomizeTypesInfo(typesInfo);
                    } catch (Exception e) {
                        throw new InvalidOperationException(
                            string.Format("Exception occurs while the '{0}' customizes XPDictionary: {1}", controller.GetType().FullName, e.Message), e);
                    }
                }
                OnCustomizeTypesInfo(XafTypesInfo.Instance);
            }
        }
        private void LoadModel(ITypesInfo typesInfo) {
            Tracing.Tracer.LockFlush();
            try {
                Tick.In("LoadModel");
                Tick.In("LoadModel.1");
                CollectStuff(typesInfo);
                Tracing.Tracer.LogSubSeparator("Load Model");
                Tracing.Tracer.LogText("Load Schema");
                scannedForModuleAssemblies.Clear();
                Tick.Out("LoadModel.1");
                Tick.In("LoadModel.2");
                SetupModules();
                Tick.In("LoadModel.3");
                LoadTypesInfo(Modules, typesInfo);
                DoCustomizeTypesInfo(typesInfo);
            } finally {
                Tracing.Tracer.ResumeFlush();
                Tick.Out("LoadModel");
            }
        }
        private bool IsTypeFromModule(ITypeInfo typeInfo) {
            return assemblyHash.ContainsKey(typeInfo.AssemblyInfo.Assembly);
        }
        private void CollectStuff(ITypesInfo typesInfo) {
            foreach (ModuleBase module in Modules) {
                Assembly assembly = module.GetType().Assembly;
                assemblyHash[assembly] = assembly;
            }
            foreach (Assembly assembly in assemblyHash.Keys) {
                typesInfo.LoadTypes(assembly);
            }
            _xpandControllersManager.CollectControllers(IsTypeFromModule);
        }
        protected override void LoadTypesInfo(IList<ModuleBase> modules, ITypesInfo typesInfo) {
            foreach (ModuleBase module in modules) {
                allDomainComponents.AddRange(module.BusinessClasses);
                allDomainComponents.AddRange(module.BusinessClassAssemblies.GetBusinessClasses());
            }
            if (security != null) {
                allDomainComponents.AddRange(security.GetBusinessClasses());
            }
            foreach (Type type in allDomainComponents) {
                typesInfo.RegisterEntity(type);
            }
        }
        protected new virtual void AddModuleWithReferencedModules(ModuleBase module) {
            AddModuleIntoList(module, Modules);
        }
        public XpandApplicationModulesManager(XpandControllersManager controllersManager, string assembliesPath, ISecurity security) {
            Init(controllersManager, assembliesPath);
            Tracing.Tracer.LogSubSeparator("");
            this.security = security;
        }
        public XpandApplicationModulesManager(ISecurity security)
            : this(new XpandControllersManager(), "", security) {
            this.security = security;
        }

        public new void Clear() {
            Init(_xpandControllersManager, _assembliesPath);
            isLoaded = false;
        }
        public new void Load(ITypesInfo typesInfo) {
            Tracing.Tracer.LogVerboseText("-> ApplicationModulesManager.Load(XPDictionary)");
            if (isLoaded)
                throw new InvalidOperationException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.ModulesHasAlreadyBeenLoaded));
            Tracing.Tracer.LogLoadedAssemblies();
            LoadModel(typesInfo);
            isLoaded = true;
            Tracing.Tracer.LogVerboseText("<- ApplicationModulesManager.Load(XPDictionary)");
        }
        public new void Load() {
            Load(XafTypesInfo.Instance);
        }
        public new void AddModule(ModuleBase module) {
            if (isLoaded) {
                throw new InvalidOperationException();
            }
            if (module == null) {
                throw new ArgumentNullException("module");
            }
            if (Modules.FindModule(module.GetType()) == null) {
                try {
                    ReflectionHelper.AddResolvePath(_assembliesPath);
                    try {
                        Modules.Add(module);
                    } finally {
                        ReflectionHelper.RemoveResolvePath(_assembliesPath);
                    }
                    OnModuleRegistered(module);
                } catch (Exception e) {
                    throw new InvalidOperationException(
                        string.Format("Exception occurs while registering {0}\r\n{1}", module.GetType().FullName, e.Message), e);
                }
            } else {
                Tracing.Tracer.LogWarning("The {0} module is registered more than once.", module.GetType().Name);
            }
        }
        public new virtual ModuleBase AddModule(Type moduleType, bool loadModuleDiffs) {
            if (Modules.FindModule(moduleType) == null) {
                ModuleBase result;
                ReflectionHelper.AddResolvePath(_assembliesPath);
                try {
                    result = loadModuleDiffs ? ModuleFactory.WithResourcesDiffs.CreateModule(moduleType) : ModuleFactory.WithEmptyDiffs.CreateModule(moduleType);
                } finally {
                    ReflectionHelper.RemoveResolvePath(_assembliesPath);
                }
                AddModule(result);
                return result;
            }
            return null;
        }


    }
}
