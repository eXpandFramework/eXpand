using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace eXpandAddIns.ModelEditor {
    public class DependedAssemblyPathResolver : MarshalByRefObject
    {
        static void GetDependentModules(string assemblyPath, List<string> assemblyPaths, IDependedAttributeInspector dependedAttributeInspector)
        {
            assemblyPaths.Add(assemblyPath);
            IEnumerable<string> assemblies = dependedAttributeInspector.GetAssemblies(assemblyPath);
            foreach (string assembly in assemblies) {
                GetDependentModules(assembly, assemblyPaths, dependedAttributeInspector);
            }
        }

        public static IEnumerable<string> GetAssemblyPaths(string assemblyPath)
        {
            AppDomain.CurrentDomain.AssemblyResolve += AppDomainOnAssemblyResolve;
            var assemblyPaths = new List<string>();
            AppDomain appDomain = GreateAppDomain();
            try
            {
                string assemblyName = Assembly.GetAssembly(typeof(DependedAttributeInspector)).GetName().Name;
                string typeName = typeof(DependedAttributeInspector).ToString();
                var dependedAttributeInspector = appDomain.CreateInstanceAndUnwrap(assemblyName, typeName) as IDependedAttributeInspector;
                GetDependentModules(assemblyPath, assemblyPaths, dependedAttributeInspector);
                assemblyPaths.Remove(assemblyPath);
            }
            finally
            {
                AppDomain.Unload(appDomain);
                AppDomain.CurrentDomain.AssemblyResolve -= AppDomainOnAssemblyResolve;
            }
            return assemblyPaths;
        }

        static AppDomain GreateAppDomain() {
            string directoryName = Path.GetDirectoryName(Assembly.GetAssembly(typeof(DependedAttributeInspector)).Location);
            var appDomainSetup = new AppDomainSetup {
                                                        ApplicationBase = directoryName,
                                                        PrivateBinPath = directoryName,
                                                        DisallowBindingRedirects = false,
                                                        DisallowCodeDownload = true
                                                    };
            return AppDomain.CreateDomain("ME_Domain", null,appDomainSetup);
        }

        static Assembly AppDomainOnAssemblyResolve(object sender, ResolveEventArgs args) {
            string location = Assembly.GetAssembly(typeof(DependedAttributeInspector)).Location;
            return Assembly.LoadFile(location);
        }
    }
}