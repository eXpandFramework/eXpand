using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using Microsoft.Win32;

namespace Xpand.ToolboxCreator {
    class Program {
        static void Main(string[] args) {
            
            var isWow64 = InternalCheckIsWow64();
            string wow = isWow64 ? @"Wow6432Node\" : null;
            var registryKeys = RegistryKeys(wow);
            
            if (args.Length == 1 && args[0] == "u") {
                DeleteXpandEntries(registryKeys);
                var assemblyFolderExKey = GetAssemblyFolderExKey(wow);
                assemblyFolderExKey.DeleteSubKeyTree("Xpand", false);
                assemblyFolderExKey.Close();
                Console.WriteLine("Unistalled");
                return;
            }
            CreateAssemblyFoldersKey(wow);
            Trace.Listeners.Add(new TextWriterTraceListener("toolboxcreator.log"));
            var version = new Version();
            var error = false;
            Tracing.Initialize(@"c:\", "4");
            foreach (var file in Directory.EnumerateFiles(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Xpand.ExpressApp*.dll")) {
                try {
                    var assembly = Assembly.LoadFrom(file);
                    if (version==new Version()) {
                        version = new Version(FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion);
                    }
                    foreach (var type in assembly.GetTypes()) {
                        var toolboxItemAttribute = type.GetCustomAttributes(typeof(ToolboxItemAttribute), true).OfType<ToolboxItemAttribute>().FirstOrDefault();
                        if (toolboxItemAttribute != null && !String.IsNullOrEmpty(toolboxItemAttribute.ToolboxItemTypeName)) {
                            Register(type, file, registryKeys);
                            Console.WriteLine("Toolbox-->" + type.FullName);
                        }
                    }
                }
                catch (Exception exception) {
                    error = true;
                    var reflectionTypeLoadException = exception as ReflectionTypeLoadException;
                    Trace.TraceError(reflectionTypeLoadException!=null?reflectionTypeLoadException.LoaderExceptions[0].Message: exception.ToString());   
                }
            }
            if (error) {
                MessageBox.Show("ToolboxCreator error");
            }
            DeleteXpandEntries(registryKeys,s => !s.Contains(version.ToString()) );

            var openSubKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\" + wow + @"Microsoft\VisualStudio\11.0\", true);
            Debug.Assert(openSubKey != null, "openSubKey != null");
            openSubKey.SetValue("ConfigurationChanged", DateTime.Now.ToFileTime(), RegistryValueKind.QWord);
        }

        static void CreateAssemblyFoldersKey(string wow) {
            var registryKeys = new[]{GetAssemblyFolderExKey(wow), Registry.LocalMachine.OpenSubKey(@"SOFTWARE\" + wow + @"Microsoft\.NETFramework\AssemblyFolders", true)};
            foreach (var registryKey in registryKeys) {
                CreateXpandKey(registryKey);    
            }
        }

        static void CreateXpandKey(RegistryKey assemblyFoldersKey) {
            if (assemblyFoldersKey != null) {
                var registryKey = assemblyFoldersKey.CreateSubKey("Xpand");
                if (registryKey != null) {
                    registryKey.SetValue(null, AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
                }
            }
        }

        static RegistryKey GetAssemblyFolderExKey(string wow) {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\" + wow + @"Microsoft\.NETFramework", true);
            string minimumClrVersion = MinimumCLRVersion(registryKey);
            if (registryKey != null) {
                var subKey = registryKey.OpenSubKey(minimumClrVersion + @"\AssemblyFoldersEx", true);
                if (subKey != null) {
                    return subKey;
                }
            }
            throw new KeyNotFoundException(minimumClrVersion + @"\AssemblyFoldersEx");
        }

        static string MinimumCLRVersion(RegistryKey registryKey) {
            return registryKey.GetSubKeyNames().First(s => s.StartsWith("v4"));
        }

        static void DeleteXpandEntries(IEnumerable<RegistryKey> keys,Func<string,bool> func=null) {
            foreach (var registryKey in keys) {
                var names = registryKey.GetSubKeyNames().Where(s => s.StartsWith("Xpand") && ((func == null) || func(s)));
                foreach (var name in names) {
                    registryKey.DeleteSubKeyTree(name);
                }
            }
        }

        static List<RegistryKey> RegistryKeys(string wow) {
            return new List<RegistryKey>{
                Registry.LocalMachine.OpenSubKey(@"SOFTWARE\" + wow + @"Microsoft\VisualStudio\11.0\ToolboxControlsInstaller\", true),
                Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\VisualStudio\11.0_Config\ToolboxControlsInstaller\", true),
                Registry.LocalMachine.OpenSubKey(@"SOFTWARE\" + wow + @"Microsoft\VisualStudio\10.0\ToolboxControlsInstaller\", true),
                Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\VisualStudio\10.0_Config\ToolboxControlsInstaller\", true)
            }.Where(key => key != null).ToList();
        }

        static void Register(Type type, string file, IEnumerable<RegistryKey> registryKeys) {
            foreach (var registryKey in registryKeys) {
                var subKey = registryKey.OpenSubKey(type.Assembly.FullName,true) ?? registryKey.CreateSubKey(type.Assembly.FullName);
                Debug.Assert(subKey != null, "subKey != null");
                subKey.SetValue("CodeBase", file);
                subKey =subKey.OpenSubKey("ItemCategories",true)?? subKey.CreateSubKey("ItemCategories");
                Debug.Assert(subKey != null, "subKey2 != null");
                var toolboxTabNameAttribute = type.GetCustomAttributes(typeof(ToolboxTabNameAttribute), false).OfType<ToolboxTabNameAttribute>().FirstOrDefault();
                if (toolboxTabNameAttribute != null)
                    subKey.SetValue(type.FullName, toolboxTabNameAttribute.TabName);
            }
        }

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process(
            [In] IntPtr hProcess,
            [Out] out bool wow64Process
        );

        static bool InternalCheckIsWow64() {
            bool is64BitProcess = (IntPtr.Size == 8);
            return is64BitProcess || InternalCheckIsWow64Core();
        }

        static bool InternalCheckIsWow64Core() {
            if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
                Environment.OSVersion.Version.Major >= 6) {
                using (Process p = Process.GetCurrentProcess()) {
                    bool retVal;
                    return IsWow64Process(p.Handle, out retVal) && retVal;
                }
            }
            return false;
        }
    }
}
