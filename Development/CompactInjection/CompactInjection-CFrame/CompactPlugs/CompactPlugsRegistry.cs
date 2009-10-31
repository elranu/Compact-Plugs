using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using CompactPlugs_Primitives;

namespace CompactPlugs
{
    class CompactPlugsRegistry
    {

        Dictionary<string, Plugin> LoadedPlugins;
        Dictionary<string, Plugin> InstalledPlugins;
        Dictionary<string, Plugin> InitialPlugins;
        Dictionary<Type, object> Outputs;
        Dictionary<Type, Plugin> LazyPlugins;
        Dictionary<string, Plugin> EasyPlugins; // son aquellos plgins q tiene los callers declarados.
    }
}
