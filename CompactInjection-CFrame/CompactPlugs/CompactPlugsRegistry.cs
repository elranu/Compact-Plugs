using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using CompactPlugs_Primitives;
using System.Collections;

namespace CompactPlugs
{
    internal class CompactPlugsRegistry
    {

        Dictionary<string, KeyValuePair<Plugin, object>> LoadedPlugins;
        Dictionary<string, Plugin> InstalledPlugins;
        Dictionary<string, Plugin> InitialPlugins;
        Dictionary<Type, object> Outputs;
        Dictionary<Type, Plugin> LazyPlugins;
        Dictionary<string, Plugin> EasyPlugins; // son aquellos plgins q tiene los callers declarados.
             
        public CompactPlugsRegistry()
        {
            LoadedPlugins = new Dictionary<string, KeyValuePair<Plugin, object>>();
            InstalledPlugins = new Dictionary<string, Plugin>();
            InitialPlugins = new Dictionary<string, Plugin>();
            Outputs = new Dictionary<Type, object>();
            LazyPlugins = new Dictionary<Type, Plugin>();
            EasyPlugins = new Dictionary<string, Plugin>();
        }

        public void Add(PlugsContainer plugs)
        { 
        
        }

        public void AddLoadedPlugin(Plugin plug, object obj)
        {
            LoadedPlugins.Add(plug.Name, new KeyValuePair<Plugin, object>(plug, obj));
        }

        public Plugin[] GetInitialPlugs()
        {
            return InitialPlugins.ToArray<Plugin>();
        }

        public bool IsPluginInstalled(Plugin plug)
        {
            return InstalledPlugins.ContainsKey(plug.Name);
        }

        

        
    }

    static class ExtensionMethods
    {
        public static TSource[] ToArray<TSource>(this IEnumerable source)
        {
            return source.Cast<TSource>().ToArray();
        }
    }
}
