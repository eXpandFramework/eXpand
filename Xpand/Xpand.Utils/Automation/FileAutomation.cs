using System;
using System.Collections;
using System.IO;

namespace Xpand.Utils.Automation {
    public class FileAutomation {
        #region delegates
        #region Nested type: moveFileDelegate
        delegate bool moveFileDelegate(string sourcePath, string destinationPath);
        #endregion
        #region Nested type: waitForChangedDelegate
        delegate WaitForChangedResult waitForChangedDelegate(
            string directoryPath, string filter, WatcherChangeTypes watcherChangeTypes, int timeOut);
        #endregion
        #endregion
        #region WaitForChangedResult
        WaitForChangedResult waitForChangedHandler(string directoryPath, string filter,
                                                   WatcherChangeTypes watcherChangeTypes, int timeOut) {
            if (watcherChangeTypes != WatcherChangeTypes.Created)
                throw new NotImplementedException(watcherChangeTypes.ToString());
            var filesList = new ArrayList(Directory.GetFiles(directoryPath, filter));

            var waitForChangedResult = new WaitForChangedResult();
            while (true) {
                var newFilesList = new ArrayList(Directory.GetFiles(directoryPath, filter));
                foreach (string file in newFilesList) {
                    if (!filesList.Contains(file)) {
                        waitForChangedResult.ChangeType = WatcherChangeTypes.Created;
                        waitForChangedResult.Name = file;
                        return waitForChangedResult;

                    }
                }
            }
        }

        /// <summary>
        /// This method is to be used for win98,winME
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="filter"></param>
        /// <param name="watcherChangeTypes"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public WaitForChangedResult WaitForChanged(string directoryPath, string filter,
                                                   WatcherChangeTypes watcherChangeTypes,
                                                   int timeOut) {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
                var watcher = new FileSystemWatcher(directoryPath, filter);
                WaitForChangedResult waitForChanged = watcher.WaitForChanged(watcherChangeTypes, timeOut);
                return waitForChanged;
            }

            waitForChangedDelegate d = waitForChangedHandler;
            IAsyncResult res = d.BeginInvoke(directoryPath, filter, watcherChangeTypes, timeOut, null, null);
            if (res.IsCompleted == false)
                res.AsyncWaitHandle.WaitOne(timeOut, false);
            return res.IsCompleted ? d.EndInvoke(res) : new WaitForChangedResult();
        }
        #endregion
        #region MoveFile
        public static bool moveFileHandler(string sourcePath, string destinationPath) {
            while (true) {
                try {
                    File.Move(sourcePath, destinationPath);
                    return true;
                } catch {
                }
            }
        }

        /// <summary>
        /// tries for a given Timeout (if its locked) to move a file 
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        /// <param name="timeOut"></param>
        public bool MoveFile(string sourcePath, string destinationPath, int timeOut) {
            moveFileDelegate moveFileDelegate = moveFileHandler;
            IAsyncResult asyncResult = moveFileDelegate.BeginInvoke(sourcePath, destinationPath, null, null);
            if (!asyncResult.IsCompleted)
                asyncResult.AsyncWaitHandle.WaitOne(timeOut, false);
            bool b = asyncResult.IsCompleted ? moveFileDelegate.EndInvoke(asyncResult) : false;
            return b;
        }

        /// <summary>
        /// tries for 5000 millisec (if its locked) to move a file 
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        public bool MoveFile(string sourcePath, string destinationPath) {
            bool b = MoveFile(sourcePath, destinationPath, 5000);
            return b;
        }
        #endregion
        public static void CopyFiles(string sourceDirectory, string destinationDirectory, string[] searchPatterns,
                                     bool overWrite) {
            foreach (string searchPattern in searchPatterns) {
                string[] files = Directory.GetFiles(sourceDirectory, searchPattern);
                foreach (string file in files) {
                    bool b = !overWrite && File.Exists(Path.Combine(destinationDirectory, Path.GetFileName(file) + ""));
                    if (!b)
                        File.Copy(file, Path.Combine(destinationDirectory, Path.GetFileName(file) + ""), overWrite);
                }
            }
        }
    }
}