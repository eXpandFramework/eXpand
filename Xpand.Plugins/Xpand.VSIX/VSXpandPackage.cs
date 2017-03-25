//------------------------------------------------------------------------------
// <copyright file="VSXpandPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Xpand.VSIX.ModelEditor;
using Xpand.VSIX.Options;

namespace Xpand.VSIX {
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
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideOptionPage(typeof(OptionsPage), "Xpand", "Settings", 100, 102, true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad("ADFC4E64-0397-11D1-9F4E-00A0C911004F")]
    [ProvideToolWindow(typeof(ModelToolWindow))]
    
    public sealed class VSXpandPackage : Package {
        private static VSXpandPackage _instance;

        /// <summary>
        /// VSXpandPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "fa1289e0-6376-4d19-98c5-9d0c90dd3283";

        /// <summary>
        /// Initializes a new instance of the <see cref="VSXpandPackage"/> class.
        /// </summary>
        public VSXpandPackage() {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
            _instance = this;
        }

        public static VSXpandPackage Instance => _instance;
        public static OptionsPage OptionsPage => Instance.GetDialogPage<OptionsPage>();

        public T GetDialogPage<T>() where T:DialogPage{
            return (T)GetDialogPage(typeof(T));
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize() {
            base.Initialize();
            Commands.Commands.Initialize(this);
        }

        #endregion
    }
}
