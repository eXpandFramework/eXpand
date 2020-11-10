using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace Xpand.VSIX.ToolWindow.FavoriteProject {
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("5303fa42-a1da-43e2-8b48-9ff57dffbe5b")]
    public sealed class FavoriteProjectToolWindow : ToolWindowPane {
        private readonly FavoriteProjectToolWindowControl _projectToolWindowControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="FavoriteProjectToolWindow"/> class.
        /// </summary>
        public FavoriteProjectToolWindow() : base(null) {
            Caption = "Favorite Projects";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            _projectToolWindowControl = new FavoriteProjectToolWindowControl();
        }

        public override IWin32Window Window => _projectToolWindowControl;
    }
}
