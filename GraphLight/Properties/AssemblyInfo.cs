using System.Resources;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Markup;

#if !SILVERLIGHT
[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]
[assembly: InternalsVisibleTo("WpfGraphLightTest")]
[assembly: InternalsVisibleTo("GraphLight.Prefomance")]
#endif

[assembly: NeutralResourcesLanguageAttribute("ru")]
[assembly: XmlnsPrefix("http://graphlight.codeplex.com", "gl")]
//[assembly: XmlnsDefinition("http://graphlight.codeplex.com", "GraphLight.Drawing", AssemblyName = "GraphLight.Drawing")]
//[assembly: XmlnsDefinition("http://graphlight.codeplex.com", "GraphLight.Converters", AssemblyName = "GraphLight.Drawing")]
//[assembly: XmlnsDefinition("http://graphlight.codeplex.com", "GraphLight.Controls", AssemblyName = "GraphLight.Drawing")]