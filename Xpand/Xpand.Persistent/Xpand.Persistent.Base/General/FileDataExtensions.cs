using System.IO;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.General {
    public static class FileDataExtensions {
        public static byte[] GetBytes(this IFileData fileData) {
            var memoryStream = new MemoryStream();
            fileData?.SaveToStream(memoryStream);
            return memoryStream.ToArray();
        }

    }
}
