using System;
using System.IO;
using System.Reflection;

namespace Xpand.Utils.Helpers {
    public static class StreamExtensions {

        public static void SaveToFile(this Stream stream, string filePath) {
            var directory = Path.GetDirectoryName(filePath) + "";
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
            using (var fileStream = File.OpenWrite(filePath)) {
                stream.CopyTo(fileStream);
            }
        }

        public static void WriteResourceToFile(this Type type, string resourceName,string filePath) {
            type.Assembly.GetManifestResourceStream(type,resourceName).SaveToFile(filePath);
        }

        public static string GetResourceString(this Assembly assembly, string name) {
            return assembly.GetManifestResourceStream(name).ReadToEndAsString();
        }

        public static string GetResourceString(this Type type, string name) {
            return type.Assembly.GetManifestResourceStream(type, name).ReadToEndAsString();
        }

        public static string GetDxScriptFromResource(this Type type, string name) {
            return type.Assembly.GetManifestResourceStream(type, name).ReadToEndAsString();
        }

        public static string ReadToEndAsString(this Stream stream) {
            using (var streamReader = new StreamReader(stream)) {
                return streamReader.ReadToEnd();

            }
        }
    }
}
