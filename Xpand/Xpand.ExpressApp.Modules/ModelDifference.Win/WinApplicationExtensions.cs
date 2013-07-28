using System.Windows.Forms;
using DevExpress.ExpressApp.Win;

namespace Xpand.ExpressApp.ModelDifference.Win {
    public static class WinApplicationExtensions {
        static WinApplication _xpandWinApplication;

        public static void HandleException(this WinApplication xpandWinApplication) {
            if (_xpandWinApplication == null)
                Application.ThreadException += (sender, args) => xpandWinApplication.HandleException(args.Exception);
            else {
                Application.ThreadException += (sender, args) => _xpandWinApplication.HandleException(args.Exception);
            }
            if (_xpandWinApplication == null)
                _xpandWinApplication = xpandWinApplication;
        }
    }
}
