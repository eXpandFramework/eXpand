using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.Utils;
using Microsoft.Win32;
using Mono.Cecil;
using Xpand.VSIX.Extensions;

namespace Xpand.VSIX.Wizard{
    public class ModuleManager{
        private readonly IList<XpandModule> _modules;

        public static XpandModule[] GetModules(Platform platform){
            return Instance.Modules.Where(module => !module.HasPlatformVersion)
                .Concat(Instance.Modules.Where(module => module.Platform==platform)).ToArray();
        }

        private static void RegisterAllModules(){
            var xpandPath = GetXpandDllPath();
            if (!Directory.Exists(xpandPath))
                MessageBox.Show(
                    @"Xpand not found check that HKLM\Sofware\Wow6432Node\Microsoft\.NetFramework\AssemblyFolders\Xpand points to the Xpand.DLL directory",
                    typeof(ModuleManager).Namespace);
            else{
                var fileNames = Directory.GetFiles(xpandPath, "Xpand.ExpressApp.*.dll");
                foreach (var fileName in fileNames){
                    Register(fileName);
                }
            }
        }

        public static string GetXpandDllPath() {
            try {
                var softwareNode = Registry.LocalMachine.OpenSubKey(@"Software\Wow6432Node") ?? Registry.LocalMachine.OpenSubKey("Software");
                return Path.GetFullPath(softwareNode?.OpenSubKey(@"Microsoft\.NetFramework\AssemblyFolders\Xpand")?.GetValue(null) + "");
            }
            catch {
                return null;
            }
        }

        ModuleManager(){
            _modules = new List<XpandModule>();
        }

        public static ModuleManager Instance { get; } = new ModuleManager();

        public static void Register(string fileName){
            var assemblyDefinition = AssemblyDefinition.ReadAssembly(fileName);
            var typeDefinition =assemblyDefinition.MainModule.Types.FirstOrDefault(
                definition =>definition.CustomAttributes.Any(attribute => attribute.AttributeType.Name == typeof(ToolboxTabNameAttribute).Name));
            if (typeDefinition != null){
                var frameworkName = System.Reflection.Assembly.LoadFile(fileName)
                    .GetCustomAttributes(typeof(TargetFrameworkAttribute), true).OfType<TargetFrameworkAttribute>()
                    .First().FrameworkName;

                var value = Regex.Match(frameworkName, @"[\d]\.[\d]\.[\d]", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline).Value;
                var xpandModule = new XpandModule(typeDefinition,new Version(value));
                Instance._modules.Add(xpandModule);
            }
        }

        public IList<XpandModule> Modules {
            get {
                if (!_modules.Any()) {
                    RegisterAllModules();
                }
                return _modules;
            }
        }
    }
}