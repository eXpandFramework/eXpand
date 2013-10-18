using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Web.UI;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an dataStoreNameSuffix.
[assembly: AssemblyTitle("Xpand.ExpressApp.Web")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Xpand.ExpressApp.Web")]
[assembly: AssemblyCopyright("Copyright © 2009")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this dataStoreNameSuffix not visible 
// to COM components.  If you need to access a type in this dataStoreNameSuffix from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("0b59f8a2-8a44-425f-8e01-eaa2156ca57a")]

// Version information for an dataStoreNameSuffix consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [dataStoreNameSuffix: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion(XpandAssemblyInfo.Version)]
[assembly: AssemblyFileVersion(XpandAssemblyInfo.FileVersion)]

[assembly: WebResource(ResourceNames.CommonStyles, "text/css")]
[assembly: WebResource(ResourceNames.jwerty, "text/javascript")]
[assembly: WebResource(ResourceNames.HighlightFocusedLayoutItem, "text/javascript")]
class ResourceNames {
    public const string CommonStyles = "Xpand.ExpressApp.Web.Styles.CommonStyle.css";
    public const string jwerty = "Xpand.ExpressApp.Web.SystemModule.WebShortcuts.jwerty.js";
    public const string HighlightFocusedLayoutItem = "Xpand.ExpressApp.Web.SystemModule.HighlightFocusedLayoutItemDetailViewController.js";
}

