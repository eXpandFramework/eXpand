using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Xpand.Utils")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Xpand.Utils")]
[assembly: AssemblyCopyright("Copyright © 2009")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("14e87e2c-acc6-45bf-bf56-865662d32223")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion(XpandAssemblyInfo.Version)]
[assembly: AssemblyFileVersion(XpandAssemblyInfo.FileVersion)]


public class XpandAssemblyInfo {
    public const string Version = "13.2.9.1";
    public const string FileVersion = Version;
    public const string Token = "c52ffed5d5ff0958";
    public const string TabAspNetModules = "eXpand: Web Modules";
    public const string TabWinModules = "eXpand: Win Modules";
    public const string TabWinWebModules = "eXpand: Win-Web Modules";
    public const string TabSecurity = "eXpand: Security";
}