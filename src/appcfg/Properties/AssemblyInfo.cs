using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("appcfg")]
[assembly: AssemblyDescription(".NET application configuration with command line parameters, config file, and environment variables made easy.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("appcfg")]
[assembly: AssemblyCopyright("Copyright © 2017 by Ralf Westphal")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("C88B2472-5DB0-4B7E-AF3B-9F677724DE89")]

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
[assembly: AssemblyVersion("1.0.2.0")]
    // increase major upon breaking changes
    // increase minor upon enhancements
    // increase build upon bug fixes
    // increase revision upon changes not affecting the behavior
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: InternalsVisibleTo("appcfg_tests")]