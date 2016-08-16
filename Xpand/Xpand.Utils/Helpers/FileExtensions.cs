using System.IO;
using System.Linq;

namespace Xpand.Utils.Helpers{
    public static class FileExtensions{
        public static bool IsFileLocked(this FileInfo file){
            FileStream stream = null;

            try{
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException){
                return true;
            }
            finally{
                stream?.Close();
            }
            return false;
        }

        public static string GetParentFolder(this DirectoryInfo directoryInfo, string folderName) {
            folderName = folderName.ToLower();
            while (directoryInfo != null && directoryInfo.GetDirectories().All(info => info.Name.ToLower() != folderName)) {
                directoryInfo = directoryInfo.Parent;
            }
            return directoryInfo?.GetDirectories().First(info => info.Name.ToLower() == folderName).FullName;
        }

    }
}