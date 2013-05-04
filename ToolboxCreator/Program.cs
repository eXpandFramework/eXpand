using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using Microsoft.Win32;

namespace Xpand.ToolboxCreator {
    class Program {
        static void Main() {
            var isWow64 = InternalCheckIsWow64();
            string wow = isWow64 ? @"Wow6432Node\" : null;
            var registryKeys = RegistryKeys(wow);
            DeleteXpandEntries(registryKeys);
            foreach (var file in Directory.EnumerateFiles(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Xpand*.dll")) {
                var assembly = Assembly.LoadFrom(file);
                foreach (var type in assembly.GetTypes()) {
                    var toolboxItemAttribute = type.GetCustomAttributes(typeof(ToolboxItemAttribute), true).OfType<ToolboxItemAttribute>().FirstOrDefault();
                    if (toolboxItemAttribute != null && !string.IsNullOrEmpty(toolboxItemAttribute.ToolboxItemTypeName)) {
                        Register(type, file, registryKeys);
                        Console.WriteLine("Toolbox-->" + type.FullName);
                    }
                }
            }
            var openSubKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\" + wow + @"Microsoft\VisualStudio\11.0\", true);
            Debug.Assert(openSubKey != null, "openSubKey != null");
            openSubKey.SetValue("ConfigurationChanged", DateTime.Now.ToFileTime(), RegistryValueKind.QWord);
        }

        static void DeleteXpandEntries(IEnumerable<RegistryKey> keys) {
            foreach (var registryKey in keys) {
                var names = registryKey.GetSubKeyNames().Where(s => s.StartsWith("Xpand"));
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
                var subKey = registryKey.CreateSubKey(type.Assembly.FullName);
                Debug.Assert(subKey != null, "subKey != null");
                subKey.SetValue("CodeBase", file);
                subKey = subKey.CreateSubKey("ItemCategories");
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
