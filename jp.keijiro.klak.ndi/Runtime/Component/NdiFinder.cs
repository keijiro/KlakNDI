using System.Collections.Generic;

namespace Klak.Ndi {

public static class NdiFinder
{
    public static IEnumerable<string> sourceNames => EnumerateSourceNames();
    public static IEnumerable<Interop.Source> sources => EnumerateSources();
    
    public static IEnumerable<string> EnumerateSourceNames()
    {
        var list = new List<string>();
        foreach (var source in SharedInstance.Find.CurrentSources)
            list.Add(source.NdiName);
        return list;
    }
    
    public static IEnumerable<Interop.Source> EnumerateSources()
    {
        var list = new List<Interop.Source>();
        foreach (var source in SharedInstance.Find.CurrentSources)
            list.Add(source);
        return list;
    }
}

} // namespace Klak.Ndi
