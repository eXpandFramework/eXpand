using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Xpand.VSIX.Extensions;
using Xpand.VSIX.ModelEditor;
using Xpand.VSIX.Options;
using Xpand.VSIX.Services;
using Task = System.Threading.Tasks.Task;

namespace Xpand.VSIX.VSPackage {
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true,AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideOptionPage(typeof(OptionsPage), "Xpand", "Settings", 100, 102, true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad("ADFC4E64-0397-11D1-9F4E-00A0C911004F",PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideToolWindow(typeof(ModelToolWindow))]
    [ProvideToolboxItems(1)]
    public sealed class VSPackage : AsyncPackage ,IDTE2Provider{
        private static VSPackage _instance;

        public const string PackageGuidString = "fa1289e0-6376-4d19-98c5-9d0c90dd3284";
        public VSPackage() {
            _instance = this;
        }

        public new object GetService(Type type) {
            return base.GetService(type);
        }

        public static VSPackage Instance => _instance;
        public static OptionsPage OptionsPage => Instance.GetDialogPage<OptionsPage>();

        public T GetDialogPage<T>() where T:DialogPage{
            return (T)GetDialogPage(typeof(T));
        }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress) {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            ExternalToolsService.Init();
            ModelMapperService.Init();
            Commands.Commands.Initialize();
        }
    }

}
