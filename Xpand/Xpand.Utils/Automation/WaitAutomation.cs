using System;
using System.Drawing;
using System.IO;
using System.Security;

namespace Xpand.Utils.Automation {
    [SecuritySafeCritical]
    public class WaitAutomation {
        public static void WaitFor(int milliSec) {
            Wait.SleepFor(milliSec);
        }

        #region WaitForFileToBeCreated
        /// <summary>
        /// default timeout is 5000 millisec
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="filter"></param>
        /// <returns>true if the file exists at the given time</returns>
        public static bool WaitForFileToBeCreated(string directoryPath, string filter) {
            return WaitForFileToBeCreated(directoryPath, 5000, filter);
        }

        /// <summary>
        /// default timeout is 5000 millisec
        /// </summary>
        /// <param name="filePath">path and name of the file</param>
        /// <returns>true if the file exists at the given time</returns>
        public static bool WaitForFileToBeCreated(string filePath) {
            return WaitForFileToBeCreated(filePath, 5000);
        }

        /// <summary>
        /// _
        /// </summary>
        /// <param name="filePath">path and name of the file</param>
        /// <param name="milliSec"></param>
        /// <returns>true if the file exists at the given time</returns>
        public static bool WaitForFileToBeCreated(string filePath, int milliSec) {
            return WaitForFileToBeCreated(Path.GetDirectoryName(filePath), milliSec, Path.GetFileName(filePath));
        }
        
        /// <summary>
        /// _
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="milliSec"></param>
        /// <param name="filter"></param>
        /// <returns>true if the file exists at the given time</returns>
        public static bool WaitForFileToBeCreated(string directoryPath, int milliSec, string filter) {
            var fileAutomation = new FileAutomation();
            WaitForChangedResult waitForChanged = fileAutomation.WaitForChanged(directoryPath, filter,
                                                                                WatcherChangeTypes.Created, milliSec);
            bool b = waitForChanged.Name + "" != "";
            return b;
        }
        #endregion
    }
}