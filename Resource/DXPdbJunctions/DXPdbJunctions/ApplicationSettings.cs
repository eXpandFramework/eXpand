using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace DXPdbJunctions{
    public class ApplicationSettings{
        private static ApplicationSettings _instance;
        private List<DXperience> _dXperienceInstalled;

        public ApplicationSettings(){
            GetDXVersionsInstalled();
        }

        public static ApplicationSettings Instance {
            get { return _instance ?? (_instance = new ApplicationSettings()); }
        }

        public List<DXperience> DXperienceInstalled{
            get { return _dXperienceInstalled; }
        }

        private void GetDXVersionsInstalled() {
            _dXperienceInstalled = new List<DXperience>();
            try{
                string wowNode = Environment.Is64BitOperatingSystem ? "Wow6432Node" : null;
                var registryKey = Registry.LocalMachine.OpenSubKey(String.Format(@"SOFTWARE\{0}\DevExpress\Components", wowNode));
                if (registryKey != null){
                    FillDXVersionsTable(registryKey);
                }
            }
            catch{
                Logger.Instance.AddText("Error in registry", TextItemStatus.Error);
            }
        }

        private void FillDXVersionsTable(RegistryKey key) {
            string[] items = key.GetSubKeyNames();
            foreach (string s in items) {
                var registryKey = key.OpenSubKey(s);
                if (registryKey != null){
                    string rootFolder = registryKey.GetValue("RootDirectory").ToString();
                    string sourceFolder = "";
                    if (System.IO.Directory.Exists(rootFolder + "\\Sources\\DevExpress.Data\\")) {
                        sourceFolder = rootFolder + "Sources\\";
                    }
                    DXperienceInstalled.Add(new DXperience{ Version = s.TrimStart('v'), RootFolder = rootFolder, SourceFolder = sourceFolder });
                }
            }
        }

        public DXperience DXperience{
            get { return DXperienceInstalled[DXperienceInstalled.Count - 1]; }
        }

        public bool CommandPromptMode { get; set; }

    }
    public class DXperience {
        public DXperience Value {
            get { return this; }
        }
        public string Version { get; set; }
        public string SourceFolder { get; set; }
        public string RootFolder { get; set; }
        public bool SourcesAvaialble {
            get { return string.IsNullOrEmpty(SourceFolder); }
        }
    }

}