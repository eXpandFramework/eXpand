using System.IO;

namespace Xpand.Utils.Helpers {
    public static class FileExtensions {
        public static bool IsFileLocked(this FileInfo file) {
            FileStream stream = null;

            try {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException) {
                return true;
            }
            finally {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }

    }
}
