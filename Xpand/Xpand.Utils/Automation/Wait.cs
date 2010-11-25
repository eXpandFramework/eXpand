using System;
using System.Threading;

namespace Xpand.Utils.Automation {
    public class Wait {
        #region private fields
        #endregion
        #region constants
        public const int defaultTimeSleep = 1000;
        #endregion
        #region delegates
        delegate void sleepDelegate(int miliSec);
        #endregion
        #region constructors
        public Wait()
            : this(defaultTimeSleep) {
        }

        public Wait(int miliSec) {
            Sleep(miliSec);
        }
        #endregion
        #region static methods
        public static void SleepFor() {
            new Wait(defaultTimeSleep);
        }

        public static void SleepFor(int miliSec) {
            new Wait(miliSec);
        }
        #endregion
        #region private methods
        void SleepHandler(int miliSec) {
            Thread.Sleep(miliSec);
        }
        #endregion
        public void Sleep(int miliSec) {
            sleepDelegate sleepDelegate = SleepHandler;
            IAsyncResult asyncResult = sleepDelegate.BeginInvoke(miliSec, null, null);
            if (!asyncResult.IsCompleted)
                asyncResult.AsyncWaitHandle.WaitOne(miliSec, false);
        }
    }
}