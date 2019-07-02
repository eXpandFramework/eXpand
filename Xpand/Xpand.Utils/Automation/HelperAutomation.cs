using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;

namespace Xpand.Utils.Automation {
    [SecuritySafeCritical]
    public class HelperAutomation {
        private ICollection<string> _allWindowCaptions;
        private ICollection<IntPtr> _allWindowHandles;

        public ICollection<string> AllWindowCaptions {
            get { return _allWindowCaptions; }
        }

        public ICollection<IntPtr> AllWindowHandles {
            get { return _allWindowHandles; }
        }

        public void KillProcesses(string processName) {
            Process[] processesByName = Process.GetProcessesByName(processName);
            foreach (Process process in processesByName) {
                process.Kill();
            }
        }
    }
}