using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevExpress.DXCore.Controls.Utils;
using Microsoft.Win32;
using Mono.Cecil;
using Xpand.VSIX.Extensions;

namespace Xpand.VSIX.Wizard{
    public class ModuleManager{
        public static XpandModule[] GetModules(Platform platform){
            return Instance.Modules.Where(module => !module.HasPlatformVersion)
                .Concat(Instance.Modules.Where(module => module.Platform==platform)).ToArray();
        }

        static ModuleManager(){
            var xpandPath = GetXpandDLLPath();
            if (!Directory.Exists(xpandPath)) {
                DteExtensions.DTE.WriteToOutput(@"Xpand not found that check HKLM\Sofware\Wow6432Node\Microsoft\.NetFramework\AssemblyFolders\Xpand points to the Xpand.DLL directory");
            }
            else{
                var fileNames = Directory.GetFiles(xpandPath, "Xpand.ExpressApp.*.dll");
                foreach (var fileName in fileNames) {
                    Register(fileName);
                }
            }
        }
        private static string GetXpandDLLPath() {
            try {
                var softwareNode = Registry.LocalMachine.OpenSubKey(@"Software\Wow6432Node") ?? Registry.LocalMachine.OpenSubKey("Software");
                return Path.GetFullPath(softwareNode?.OpenSubKey(@"Microsoft\.NetFramework\AssemblyFolders\Xpand")?.GetValue(null) + "");
            }
            catch {
                return null;
            }
        }

        ModuleManager(){
            Modules = new List<XpandModule>();
        }

        public static ModuleManager Instance { get; } = new ModuleManager();

        public static void Register(string fileName){
            var assemblyDefinition = AssemblyDefinition.ReadAssembly(fileName);
            var typeDefinition =assemblyDefinition.MainModule.Types.FirstOrDefault(
                definition =>definition.CustomAttributes.Any(attribute => attribute.AttributeType.Name == typeof(ToolboxTabNameAttribute).Name));
            if (typeDefinition != null){
                
                var xpandModule = new XpandModule(typeDefinition);
                Instance.Modules.Add(xpandModule);
            }
        }

        public IList<XpandModule> Modules { get; }
    }
}