using System;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq;

namespace GacInstaller {
    internal class Program {
        // Methods
        static void Main(string[] args) {
            string pattern = "";
            if (args.Length > 0) {
                pattern = args[0];
            }
            
            bool flag = true;
            if (args.Length == 2) {
                flag = args[1] == "i";
            }
            var publish = new Publish();
            bool error=false;
            foreach (var file in GetFiles()) {
                try {
                    if (Regex.IsMatch(Path.GetFileNameWithoutExtension(file)+"", pattern)) {
                        if (flag) {
                            publish.GacInstall(file);
                            Console.WriteLine("Installed: " + file);
                        }
                        else {
                            var windowsPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                            var path = windowsPath + @"\Microsoft.NET\assembly\GAC_MSIL\" + Path.GetFileNameWithoutExtension(file);
                            if (Directory.Exists(path)) {
                                Directory.Delete(path,true);
                                Console.WriteLine("Uninstalled: " + Path.GetFileNameWithoutExtension(file) );
                            }
                            else {
                                Console.WriteLine("Already uninstalled: " + Path.GetFileNameWithoutExtension(file));
                            }
                        }
                    }
                }
                catch (Exception exception) {
                    error = true;
                    var foregroundColor = Console.ForegroundColor;
                    Console.ForegroundColor=ConsoleColor.Red;
                    Console.WriteLine(new Exception("ERROR in " + Path.GetFileNameWithoutExtension(file), exception));
                    Console.ForegroundColor=foregroundColor;
                }
            }
            if (error)
                Console.ReadKey();
        }

        static IEnumerable<string> GetFiles() {
            return Directory.GetFiles(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "*.dll").Where(IsSigned);
        }

        private static bool IsSigned(string s){
            try{
                return Assembly.ReflectionOnlyLoadFrom(s).GetName().GetPublicKeyToken().Length>0;
            }
            catch (BadImageFormatException){
                return false;
            }
        }
    }
}