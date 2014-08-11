using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp.Utils;

namespace Xpand.Persistent.Base.ModelDifference {
    public class ResourceModelCollector {
        private const int MaxExpectedEncodingStringLengthInBytes = 512;
        private static readonly Encoding[] _expectedEncodings = { Encoding.UTF8, Encoding.ASCII, Encoding.Unicode, Encoding.UTF7, Encoding.UTF32, Encoding.BigEndianUnicode };
        private static readonly Encoding _defaultEncoding = Encoding.UTF8;
        public Dictionary<string, ResourceInfo> Collect(IEnumerable<Assembly> assemblies, string prefix){
            var assemblyResourcesNames = assemblies.SelectMany(assembly => assembly.GetManifestResourceNames().Where(s => s.EndsWith(".xafml")), (assembly1, s) => new { assembly1, s });
            if (!string.IsNullOrEmpty(prefix))
                assemblyResourcesNames = assemblyResourcesNames.Where(arg => ((arg.s.StartsWith(prefix) || (!(arg.s.StartsWith(prefix)) && arg.s.IndexOf("." + prefix, StringComparison.Ordinal) > -1))));
            var dictionary = new Dictionary<string, ResourceInfo>();
            foreach (var assemblyResourcesName in assemblyResourcesNames){
                var resourceName = assemblyResourcesName.s;
                string path = GetPath(prefix, resourceName);
                resourceName = GetResourceName(prefix, path);
                if (!(dictionary.ContainsKey(resourceName)))
                    dictionary.Add(resourceName, new ResourceInfo(resourceName, assemblyResourcesName.assembly1));
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
            return resourceName.StartsWith(prefix) ? resourceName : resourceName.Substring(resourceName.IndexOf("." + prefix, StringComparison.Ordinal) + 1);
        }

        string GetAspectName(string resourceName) {
            var regexObj = new Regex(@"_(..)\.xafml");
            Match matchResults = regexObj.Match(resourceName);
            if (!(matchResults.Success))
                return "";
            return matchResults.Groups[1].Value;
        }

        string GetXml(string resourceName, Assembly assembly1) {
            string readToEnd;
            using (Stream manifestResourceStream = assembly1.GetManifestResourceStream(resourceName)) {
                if (manifestResourceStream == null) throw new NullReferenceException(resourceName);
                Encoding encoding = GetStreamEncoding(manifestResourceStream) ?? _defaultEncoding;
                using (var streamReader = new StreamReader(manifestResourceStream, encoding)) {
                    readToEnd = streamReader.ReadToEnd();
                }
            }
            return readToEnd;
        }

        Encoding GetEncodingFromHeader(string encodingString) {
            if (string.IsNullOrEmpty(encodingString)) return null;
            int start = encodingString.IndexOf(@"encoding=""", StringComparison.Ordinal);
            if (start >= 0) {
                start += 10;
                int end = encodingString.IndexOf(@"""", start, StringComparison.Ordinal);
                if (end > 0) {
                    string encodingStr = encodingString.Substring(start, end - start);
                    try {
                        return Encoding.GetEncoding(encodingStr);
                    }
                    catch {
                        return null;
                    }
                }
            }
            return null;
        }

        internal Encoding GetStreamEncoding(Stream stream) {
            Guard.ArgumentNotNull(stream, "stream");
            if (stream.Length == 0) return null;
            var bytes = new byte[Math.Min(stream.Length, MaxExpectedEncodingStringLengthInBytes)];
            long position = stream.Position;
            try {
                stream.Position = 0;
                stream.Read(bytes, 0, bytes.Length);
            }
            finally {
                stream.Position = position;
            }
            foreach (Encoding encoding in _expectedEncodings) {
                string content = encoding.GetString(bytes);
                Encoding result = GetEncodingFromHeader(content);
                if (result == null) continue;
                if (ReferenceEquals(result , encoding)) return result;
                content = result.GetString(bytes);
                if (GetEncodingFromHeader(content) == null) {
                    throw new InvalidOperationException(
                        string.Format(
                            "Encoding '{0}' manually specified in stream does not match actual stream encoding '{1}'.",
                            result.HeaderName, encoding.HeaderName));
                }
                return result;
            }
            return null;
        }

    }
}