using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Xpand.Persistent.Base.ModelDifference {
    public class ResourceModelCollector {
        public Dictionary<string, ResourceInfo> Collect(IEnumerable<Assembly> assemblies, string prefix){
            var assemblyResourcesNames = assemblies.SelectMany(assembly => assembly.GetManifestResourceNames().Where(s => s.EndsWith(".xafml")), (assembly1, s) => new { assembly1, s });
            if (!string.IsNullOrEmpty(prefix))
                assemblyResourcesNames = assemblyResourcesNames.Where(arg => ((arg.s.StartsWith(prefix) || (!(arg.s.StartsWith(prefix)) && arg.s.IndexOf("." + prefix) > -1))));
            var dictionary = new Dictionary<string, ResourceInfo>();
            foreach (var assemblyResourcesName in assemblyResourcesNames){
                var resourceName = assemblyResourcesName.s;
                string path = GetPath(prefix, resourceName);
                resourceName = GetResourceName(prefix, path);
                if (!(dictionary.ContainsKey(resourceName)))
                    dictionary.Add(resourceName, new ResourceInfo(resourceName, new AssemblyName(assemblyResourcesName.assembly1.FullName).Name));
                var assembly1 = assemblyResourcesName.assembly1;
                var xml = GetXml(assemblyResourcesName.s, assembly1);
                string aspectName = GetAspectName(assemblyResourcesName.s);
                dictionary[resourceName].AspectInfos.Add(new AspectInfo(xml,aspectName));
            }
            return dictionary;
        }

        string GetResourceName(string prefix, string path) {
            path = Regex.Replace(path, @"(_..)\.xafml", ".xafml");
            string resourceName = (Path.GetFileNameWithoutExtension(path) + "");
            if (string.IsNullOrEmpty(prefix))
                return resourceName;
            return resourceName.Replace(prefix, "");
        }

        string GetPath(string prefix, string resourceName) {
            if (string.IsNullOrEmpty(prefix))
                return resourceName;
            return resourceName.StartsWith(prefix) ? resourceName : resourceName.Substring(resourceName.IndexOf("." + prefix) + 1);
        }

        string GetAspectName(string resourceName) {
            var regexObj = new Regex(@"_(..)\.xafml");
            Match matchResults = regexObj.Match(resourceName);
            if (!(matchResults.Success))
                return "";
            return matchResults.Groups[1].Value;
        }

        string GetXml(string resourceName, Assembly assembly1)
        {
            string readToEnd;
            using (var manifestResourceStream = assembly1.GetManifestResourceStream(resourceName))
            {
                if (manifestResourceStream == null) throw new NullReferenceException(resourceName);
                using (var streamReader = new StreamReader(manifestResourceStream))
                {
                    readToEnd = streamReader.ReadToEnd();
                }
            }
            return readToEnd;
        }

    }
}