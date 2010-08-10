using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace eXpand.Utils.DependentAssembly {
    public class DependentAssemblyPathResolver : MarshalByRefObject
    {
        static void GetDependentModules(string assemblyPath, List<string> assemblyPaths, IDependentAssemblyInspector dependentAssemblyInspector)
        {
            assemblyPaths.Add(assemblyPath);
            IEnumerable<string> assemblies = dependentAssemblyInspector.GetAssemblies(assemblyPath);
            foreach (string assembly in assemblies) {
                GetDependentModules(assembly, assemblyPaths, dependentAssemblyInspector);
            }
        }

        public static IEnumerable<string> GetAssemblyPaths(string assemblyPath)
        {
            AppDomain.CurrentDomain.AssemblyResolve += AppDomainOnAssemblyResolve;
            var assemblyPaths = new List<string>();
            AppDomain appDomain = GreateAppDomain();
            try
            {
                string assemblyName = Assembly.GetAssembly(typeof(DependentAttributeInspector)).GetName().Name;
                string typeName = typeof(DependentAttributeInspector).ToString();
                var dependedAttributeInspector = appDomain.CreateInstanceAndUnwrap(assemblyName, typeName) as IDependentAssemblyInspector;
                GetDependentModules(assemblyPath, assemblyPaths, dependedAttributeInspector);
            }
            finally
            {
                AppDomain.Unload(appDomain);
                AppDomain.CurrentDomain.AssemblyResolve -= AppDomainOnAssemblyResolve;
            }
            return assemblyPaths;
        }

        static AppDomain GreateAppDomain() {
            string directoryName = Path.GetDirectoryName(Assembly.GetAssembly(typeof(DependentAttributeInspector)).Location);
            var appDomainSetup = new AppDomainSetup {
                                                        ApplicationBase = directoryName,
                                                        PrivateBinPath = directoryName,
                                                        DisallowBindingRedirects = false,
                                                        DisallowCodeDownload = true
                                                    };
            return AppDomain.CreateDomain("ME_Domain", null,appDomainSetup);
        }

        static Assembly AppDomainOnAssemblyResolve(object sender, ResolveEventArgs args) {
            string location = Assembly.GetAssembly(typeof(DependentAttributeInspector)).Location;
            return Assembly.LoadFile(location);
        }
    }
}