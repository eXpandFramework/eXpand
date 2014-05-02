using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;

namespace Xpand.Utils.Proxies{
    public class AssemblyReflectionProxy : MarshalByRefObject{
        private string _assemblyPath;

        public void LoadAssembly(string assemblyPath, bool reflectionOnly=false){
            try{
                _assemblyPath = assemblyPath;
                if (reflectionOnly)
                    Assembly.ReflectionOnlyLoadFrom(assemblyPath);
                else{
                    Assembly.LoadFrom(assemblyPath);
                }
            }
            catch (FileNotFoundException){
                // Continue loading assemblies even if an assembly can not be loaded in the new AppDomain.
            }
        }

        public TResult Reflect<TResult>(Func<Assembly, TResult> func,bool reflectionOnly){
            DirectoryInfo directory = new FileInfo(_assemblyPath).Directory;
            var assemblies =reflectionOnly? AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies():AppDomain.CurrentDomain.GetAssemblies();
            ResolveEventHandler resolveEventHandler = (s, e) => OnAssemblyResolve(e, directory, assemblies,reflectionOnly);
            if (reflectionOnly)
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += resolveEventHandler;
            else{
                AppDomain.CurrentDomain.AssemblyResolve+=resolveEventHandler;
            }

            var assembly = assemblies.FirstOrDefault(a => String.Compare(a.Location, _assemblyPath, StringComparison.Ordinal) == 0);

            TResult result = func(assembly);

            if (reflectionOnly)
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= resolveEventHandler;
            else{
                AppDomain.CurrentDomain.AssemblyResolve-=resolveEventHandler;
            }

            return result;
        }


        private Assembly OnAssemblyResolve(ResolveEventArgs args, DirectoryInfo directory, IEnumerable<Assembly> reflectionOnlyGetAssemblies,bool reflectionOnly){
            Assembly loadedAssembly =reflectionOnlyGetAssemblies.FirstOrDefault(asm => string.Equals(asm.FullName, args.Name,StringComparison.OrdinalIgnoreCase));
            if (loadedAssembly != null){
                return loadedAssembly;
            }
            var assemblyName =new AssemblyName(args.Name);
            string dependentAssemblyFilename =Path.Combine(directory.FullName,assemblyName.Name + ".dll");
            string name = args.Name;
            if (File.Exists(dependentAssemblyFilename)){
                name=dependentAssemblyFilename;
            }
            return reflectionOnly?Assembly.ReflectionOnlyLoad(name):Assembly.LoadFrom(name);
        }

    }

    public class AssemblyReflectionManager : IDisposable{
        private readonly Dictionary<string, AppDomain> _loadedAssemblies = new Dictionary<string, AppDomain>();
        private readonly Dictionary<string, AppDomain> _mapDomains = new Dictionary<string, AppDomain>();

        private readonly Dictionary<string, AssemblyReflectionProxy> _proxies =
            new Dictionary<string, AssemblyReflectionProxy>();

        public void Dispose(){
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool LoadAssembly(string assemblyPath, string domainName,bool reflectionOnly=false){
            // if the assembly file does not exist then fail
            if (!File.Exists(assemblyPath))
                return false;

            // if the assembly was already loaded then fail
            if (_loadedAssemblies.ContainsKey(assemblyPath)){
                return false;
            }

            // check if the appdomain exists, and if not create a new one
            AppDomain appDomain;
            if (_mapDomains.ContainsKey(domainName)){
                appDomain = _mapDomains[domainName];
            }
            else{
                appDomain = CreateChildDomain(AppDomain.CurrentDomain, domainName);
                _mapDomains[domainName] = appDomain;
            }

            // load the assembly in the specified app domain
            try{
                Type proxyType = typeof (AssemblyReflectionProxy);
                var proxy = (AssemblyReflectionProxy)appDomain.CreateInstanceFrom(proxyType.Assembly.Location, proxyType.FullName).Unwrap();

                proxy.LoadAssembly(assemblyPath,reflectionOnly);

                _loadedAssemblies[assemblyPath] = appDomain;
                _proxies[assemblyPath] = proxy;

                return true;
            }
            catch{
            }

            return false;
        }

        public bool UnloadAssembly(string assemblyPath){
            if (!File.Exists(assemblyPath))
                return false;

            // check if the assembly is found in the internal dictionaries
            if (_loadedAssemblies.ContainsKey(assemblyPath) &&
                _proxies.ContainsKey(assemblyPath)){
                // check if there are more assemblies loaded in the same app domain; in this case fail
                AppDomain appDomain = _loadedAssemblies[assemblyPath];
                int count = _loadedAssemblies.Values.Count(a => a == appDomain);
                if (count != 1)
                    return false;

                try{
                    // remove the appdomain from the dictionary and unload it from the process
                    _mapDomains.Remove(appDomain.FriendlyName);
                    AppDomain.Unload(appDomain);

                    // remove the assembly from the dictionaries
                    _loadedAssemblies.Remove(assemblyPath);
                    _proxies.Remove(assemblyPath);

                    return true;
                }
                catch{
                }
            }

            return false;
        }

        public bool UnloadDomain(string domainName){
            // check the appdomain name is valid
            if (string.IsNullOrEmpty(domainName))
                return false;

            // check we have an instance of the domain
            if (_mapDomains.ContainsKey(domainName)){
                try{
                    AppDomain appDomain = _mapDomains[domainName];

                    // check the assemblies that are loaded in this app domain
                    var assemblies = (from kvp in _loadedAssemblies where kvp.Value == appDomain select kvp.Key).ToList();

                    // remove these assemblies from the internal dictionaries
                    foreach (string assemblyName in assemblies){
                        _loadedAssemblies.Remove(assemblyName);
                        _proxies.Remove(assemblyName);
                    }

                    // remove the appdomain from the dictionary
                    _mapDomains.Remove(domainName);

                    // unload the appdomain
                    AppDomain.Unload(appDomain);

                    return true;
                }
                catch{
                }
            }

            return false;
        }

        public TResult Reflect<TResult>(string assemblyPath, Func<Assembly, TResult> func, bool reflectionOnly=false){
            // check if the assembly is found in the internal dictionaries
            if (_loadedAssemblies.ContainsKey(assemblyPath) &&
                _proxies.ContainsKey(assemblyPath)){
                return _proxies[assemblyPath].Reflect(func,reflectionOnly);
            }

            return default(TResult);
        }

        ~AssemblyReflectionManager(){
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing){
            if (disposing){
                foreach (AppDomain appDomain in _mapDomains.Values)
                    AppDomain.Unload(appDomain);

                _loadedAssemblies.Clear();
                _proxies.Clear();
                _mapDomains.Clear();
            }
        }

        private AppDomain CreateChildDomain(AppDomain parentDomain, string domainName){
            var evidence = new Evidence(parentDomain.Evidence);
            AppDomainSetup setup = parentDomain.SetupInformation;
            return AppDomain.CreateDomain(domainName, evidence, setup);
        }
    }
}