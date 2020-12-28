using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using Xpand.VSIX.ToolWindow.FavoriteProject;

namespace Xpand.VSIX.ToolWindow.SolutionConfiguration {
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
    [Guid("0f9b57d2-21ad-4cc0-9bdb-dc3bcf00f07f")]
    public sealed class SolutionConfigurationToolWindow : ToolWindowPane {
        private readonly SolutionConfigurationToolWindowControl _projectToolWindowControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="FavoriteProjectToolWindow"/> class.
        /// </summary>
        public SolutionConfigurationToolWindow() : base(null) {
            Caption = "Solution Configuration";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            _projectToolWindowControl = new SolutionConfigurationToolWindowControl();
        }

        public override IWin32Window Window => _projectToolWindowControl;
    }
}
