using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using Xpand.Utils.Helpers;

namespace XpandAddIns {
//    public class ProjectPathBuilder {
//        public const string ProjectPaths = "ProjectPaths";
//
//        public void BuildPaths(IEnumerable<Options.SourceCodeInfo> sourceCodeInfos) {
//            sourceCodeInfos.Where(info => info.SearchOnInitialize) .Each(StoreProjectPaths);
//            var paths = GetInstallationPathsFromRegistry(ValidatePath());
//            paths.Each(StoreProjectPathsCore);
//        }
//
//        void StoreProjectPaths(Options.SourceCodeInfo sourceCodeInfo) {
//            IEnumerable<string> paths = Directory.GetFiles(sourceCodeInfo.RootPath, GetSearchPattern(),SearchOption.AllDirectories).Where(s => Regex.IsMatch(s, sourceCodeInfo.Regex));
//            Options.Storage.WriteStrings(ProjectPaths, sourceCodeInfo.Regex, paths.ToArray());
//        }
//
//        void FindProjects(Options.SourceCodeInfo sourceCodeInfo) {
//            return ;
//        }
//
///*
//        void StoreProjectPathsCore(KeyValuePair<string, string> pair) {
//            var strings = Directory.GetFiles(pair.Value, GetSearchPattern(), SearchOption.AllDirectories);
//            Options.Storage.WriteStrings(ProjectPaths, pair.Key, strings);
//        }
//*/
//
//        string GetSearchPattern() {
//            return "*.csproj" ;
//        }
//
///*
//        Func<KeyValuePair<string, string>, bool> ValidatePath() {
//            return pair => Directory.Exists(pair.Value)&&Options.Storage.ReadStrings(ProjectPaths,pair.Key).Length==0;
//        }
//*/
//
///*
//        IEnumerable<KeyValuePair<string, string>> GetInstallationPathsFromRegistry(Func<KeyValuePair<string, string>, bool> validatePath) {
//            var xafPathsFromRegistry = GetXAFPathsFromRegistry(validatePath);
//            AddXpandPath(xafPathsFromRegistry);
//            return xafPathsFromRegistry;
//        }
//*/
//
///*
//        void AddXpandPath(List<KeyValuePair<string, string>> xafPathsFromRegistry) {
//            var openSubKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\.NETFramework\AssemblyFolders\Xpand");
//            if (openSubKey != null) {
//                var value = openSubKey.GetValue(openSubKey.GetValueNames()[0]) +"";
//                if (Directory.Exists(value)) {
//                    xafPathsFromRegistry.Add(new KeyValuePair<string, string>(null, Directory.GetParent(value).FullName));
//                }
//            }
//        }
//*/
//
///*
//        List<KeyValuePair<string, string>> GetXAFPathsFromRegistry(Func<KeyValuePair<string, string>, bool> validatePath) {
//            RegistryKey openSubKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\DevExpress\eXpressApp Framework");
//            return openSubKey != null
//                       ? openSubKey.GetSubKeyNames().Select(
//                           s => new KeyValuePair<string, string>(s, GetPath(openSubKey, s))).Where(validatePath).ToList()
//                       : new List<KeyValuePair<string, string>>();
//        }
//*/
//
///*
//        string GetPath(RegistryKey openSubKey, string s) {
//            RegistryKey registryKey = openSubKey.OpenSubKey(s);
//            return (string) (registryKey != null ? registryKey.GetValue("RootDirectory") : "");
//        }
//*/
//
//        public static bool ProjectExists(EnvDTE.Project project) {
//            
//            IEnumerable<string> allProjectPaths =
//                Options.Storage.GetGroupedKeys(ProjectPaths).SelectMany(
//                    s => Options.Storage.ReadStrings(ProjectPaths, s));
//            return allProjectPaths.Where(s => s == project.FileName).FirstOrDefault() != null;
//        }
//    }
}