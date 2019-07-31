using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.Utils;
using Mono.Cecil;
using PropertyChanged;
using Xpand.VSIX.Extensions;

namespace Xpand.VSIX.Wizard{
    [AddINotifyPropertyChangedInterface]
    public class XpandModule{
        
        public bool IsSecurity { get; }
        public string Module { get; }

        [Browsable(false)]
        public TypeDefinition TypeDefinition { get; }

        public Version DotNetVersion{ get; }


        public XpandModule(TypeDefinition typeDefinition, Version dotNetVersion){
            var toolboxAttribute = typeDefinition.CustomAttributes.First(
                    attribute => attribute.AttributeType.Name == typeof(ToolboxTabNameAttribute).Name);
            var platform = toolboxAttribute.ConstructorArguments[0].Value.ToString();
            if (!platform.Contains("Win-Web"))
                Platform = platform.Contains("Win") ? Platform.Win : Platform.Web;
            Module = Regex.Replace(typeDefinition.Name, "(Xpand)?((?<name>.*))(WindowsFormsModule)|(WinModule)|(WindowsModule)|(Xpand)|(AspNet)|(Web)|(Module)", "${name}", RegexOptions.IgnoreCase);
            TypeDefinition = typeDefinition;
            DotNetVersion = dotNetVersion;
            IsSecurity = typeDefinition.Name.Contains("Security");
            if (IsSecurity)
                Install = true;
            IsWorldCreator = typeDefinition.Name.Contains("WorldCreator");
            AssemblyPath = TypeDefinition.Module.FileName;
            IsModelDifference = Path.GetFileNameWithoutExtension(AssemblyPath)== "Xpand.ExpressApp.ModelDifference";
        }

        public Platform Platform { get; }

        [Browsable(false)]
        public bool HasPlatformVersion{
            get{
                if (Platform == Platform.Web || Platform == Platform.Win)
                    return true;
                var assemblyName = TypeDefinition.Module.Assembly.Name.Name;
                return ModuleManager.Instance.Modules.Any(module => module.Platform != Platform.Agnostic &&
                                                                    module.TypeDefinition.Module.Assembly.Name.Name.Contains(assemblyName));
            }        
        }

        public string AssemblyPath { get; }
        public override string ToString(){
            return Module;
        }

        public bool Install { get; set; }
        public bool IsWorldCreator { get; }
        public bool IsModelDifference { get; }

        public XpandModule AgnosticVersion{
            get{
                if (Platform == Platform.Agnostic)
                    return this;
                var path = AssemblyPath.Replace(".Win.",".").Replace(".Web.",".");
                return ModuleManager.Instance.Modules.FirstOrDefault(module => module.AssemblyPath==path);
            }
        }
    }
    
}