using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using CompactPlugs_Primitives;
using System.Collections;
using CompactInjection;

//Develop by Mariano Julio Vicario -
//http://compactplugs.codeplex.com/
//http://www.ranu.com.ar (Blog)
//Microsoft Public License (Ms-PL)
namespace CompactPlugs
{
    internal class CompactPlugsRegistry
    {

        Dictionary<string, KeyValuePair<Plugin, object>> LoadedPlugins;
        Dictionary<Type, KeyValuePair<Plugin, object>> LoadedPluginsByType; //podria ser de que c/type no es unico. Tonces deberia de tener una List de plugins 
        Dictionary<string, Plugin> InstalledPlugins;
        Dictionary<string, Plugin> InitialPlugins;
        Dictionary<Type, object> Outputs;
        Dictionary<Type, List<Plugin>> Inputs; //sirve para poner los inputs que necesita c/plugin. Ayuda a buscar todos los plugins que tienen que ser llamados
        Dictionary<string, Plugin> AllPlugins; //k=name // son aquellos plgins q tiene los callers declarados.
        CompactConstructor _Constructor;
             
        public CompactPlugsRegistry(CompactConstructor constructor )
        {
            LoadedPlugins = new Dictionary<string, KeyValuePair<Plugin, object>>();
            LoadedPluginsByType = new Dictionary<Type, KeyValuePair<Plugin, object>>();
            InstalledPlugins = new Dictionary<string, Plugin>();
            InitialPlugins = new Dictionary<string, Plugin>();
            Outputs = new Dictionary<Type, object>();
            Inputs = new Dictionary<Type, List<Plugin>>();
            AllPlugins = new Dictionary<string, Plugin>();
            _Constructor = constructor;
        }

        public void Add(PlugsContainer plugs)
        {
            if (plugs.Plugins != null && plugs.Plugins.Length > 0)
            {
                foreach (Plugin plug in plugs.Plugins)
                {
                    this.Add(plug);
                }
            }
        }

        public void Add(Plugin plug) 
        {
            if (AllPlugins.ContainsKey(plug.Name))
            {
                AllPlugins.Add(plug.Name, plug);
                ExtractInputs(plug);
            }
        }
        /// <summary>
        /// Extract inputs metadata of the plugins
        /// </summary>
        /// <param name="plug"></param>
        private void ExtractInputs(Plugin plug)
        {
            if (plug.Inputs.Length > 0)
            {
                foreach (PluginInput input in plug.Inputs)
                {
                    Type inputType = Type.GetType(input.Type);
                    if (!Inputs.ContainsKey(inputType))
                        Inputs[inputType] = new List<Plugin>();
                    Inputs[inputType].Add(plug);
                }
            }
        }
        /// <summary>
        /// El key si esta andando el plugin o no. El Value es el plugin
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Si el value es null el plugin no existe</returns>
        public KeyValuePair<bool,Plugin> SearchPlugin(string name)
        {

            if (LoadedPlugins.ContainsKey(name))
                return new KeyValuePair<bool, Plugin>(true, LoadedPlugins[name].Key);
            if (InitialPlugins.ContainsKey(name))
                return new KeyValuePair<bool, Plugin>( false, InitialPlugins[name]);
            if (InstalledPlugins.ContainsKey(name))
                return new KeyValuePair<bool, Plugin>( false,InstalledPlugins[name]);
            if (AllPlugins.ContainsKey(name))
                return new KeyValuePair<bool, Plugin>( false, AllPlugins[name]);
            return new KeyValuePair<bool, Plugin>( false, null);
        
        }

        public void AddLoadedPlugin(Plugin plug, object obj)
        {
            if (!IsPluginLoaded(obj.GetType()))
            {
                LoadedPlugins.Add(plug.Name, new KeyValuePair<Plugin, object>(plug, obj));
                LoadedPluginsByType.Add(obj.GetType(), new KeyValuePair<Plugin, object>(plug, obj));
            }
            else
                throw new Exception(string.Format("The plugin %1 is already loaded. You cannot load two or more times the same plugin", plug.Name));
        }

        public void ExtractOutputs(object obj) 
        { 
            if(LoadedPluginsByType.ContainsKey(obj.GetType()))
            {
                Plugin plug = LoadedPluginsByType[obj.GetType()].Key;
                foreach (PluginOutput item in plug.Outputs)
                {
                    System.Reflection.PropertyInfo prop = obj.GetType().GetProperty(item.GetterProperty);
                    object output = prop.GetValue(obj, null);
                    Outputs.Add(output.GetType(), output);
                }
            }
        }
       

        public Plugin[] GetInitialPlugs()
        {
            return InitialPlugins.ToArray<Plugin>();
        }
        public bool IsPluginLoaded(string  plugname)
        {
            return LoadedPlugins.ContainsKey(plugname);
        }

        public bool IsPluginLoaded(Type ty)
        {
            return LoadedPluginsByType.ContainsKey(ty);
        }

        public bool IsPluginInstalled(Plugin plug)
        {
            return InstalledPlugins.ContainsKey(plug.Name);
        }

        public List<Plugin> GetCalledPlugins(Type ty)
        {
            //Todo: mal falta mandar todos los plugins que se deben instanciar. Tienen q estar instalados, y 
            Plugin plg = LoadedPluginsByType[ty].Key;
            return null;//AllPlugins[plg.Name];
        }

      
    }

    internal enum PluginType
    {
        Installed, Loaded, Initial, Lazy, Outputs, Easy
    }
        
    static class ExtensionMethods
    {
        public static TSource[] ToArray<TSource>(this IEnumerable source)
        {
            return source.Cast<TSource>().ToArray();
        }
    }
}
