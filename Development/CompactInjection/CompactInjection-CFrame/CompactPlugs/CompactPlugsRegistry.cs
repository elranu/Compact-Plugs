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
    internal class CompactPlugsRegistry : IDisposable
    {

        Dictionary<string, KeyValuePair<Plugin, object>> LoadedPlugins;
        Dictionary<Type, KeyValuePair<Plugin, object>> LoadedPluginsByType; //podria ser de que c/type no es unico. Tonces deberia de tener una List de plugins 
        Dictionary<string, Plugin> InstalledPlugins;
        Dictionary<string, Plugin> InitialPlugins;
        Dictionary<Type, List<KeyValuePair<string, object>>> Outputs; //Type: of the object - String: name of the plugin - Object: output 
        Dictionary<Type, List<Plugin>> Inputs; //sirve para poner los inputs que necesita c/plugin. Ayuda a buscar todos los plugins que tienen que ser llamados
        Dictionary<string, Plugin> AllPlugins; //k=name // son aquellos plgins q tiene los callers declarados.
        Dictionary<string, List<string>> PosibleCallers; //key=Caller Plugin name, Value= plugins name to initialice
        CompactConstructor _Constructor;
        Dictionary<string, List<Plugin>> CategoryDic; //key Category
             
        public CompactPlugsRegistry(CompactConstructor constructor )
        {
            LoadedPlugins = new Dictionary<string, KeyValuePair<Plugin, object>>();
            LoadedPluginsByType = new Dictionary<Type, KeyValuePair<Plugin, object>>();
            InstalledPlugins = new Dictionary<string, Plugin>(); //TODO: falta guardar el estado y loader el estado de los plugin instalados en un archivo
            InitialPlugins = new Dictionary<string, Plugin>();
            Outputs = new Dictionary<Type, List<KeyValuePair<string, object>>>();
            Inputs = new Dictionary<Type, List<Plugin>>();
            AllPlugins = new Dictionary<string, Plugin>();
            PosibleCallers = new Dictionary<string, List<string>>();
            CategoryDic = new Dictionary<string, List<Plugin>>();
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
            if (!AllPlugins.ContainsKey(plug.Name) && !InitialPlugins.ContainsKey(plug.Name))
            {
                if (plug.LazyLoad)
                {//sino es un Lazy es un initial plugin
                    AllPlugins.Add(plug.Name, plug);
                    ExtractPosibleCallers(plug);
                }
                else
                    InitialPlugins.Add(plug.Name, plug);
            }
            else
            {
                if (AllPlugins.ContainsKey(plug.Name) && plug.LazyLoad)
                    AllPlugins[plug.Name] = plug;
                else
                    InitialPlugins[plug.Name] = plug;
            }
            if (plug != null && !string.IsNullOrEmpty(plug.Category))
            {
                if(!CategoryDic.ContainsKey(plug.Category))
                    CategoryDic.Add(plug.Category, new List<Plugin>());
                CategoryDic[plug.Category].Add(plug);
            }

        }

        public List<Plugin> SearchPluginsByCategory(string category)
        {
            if (CategoryDic.ContainsKey(category))
                return CategoryDic[category];
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plug"></param>
        private void ExtractPosibleCallers(Plugin plug)
        {
            if (plug.CallerPlugins != null && plug.CallerPlugins.Length > 0)
            {
                foreach (CallerPlugin callerPlug in plug.CallerPlugins)
                {
                    if (!PosibleCallers.ContainsKey(callerPlug.name))
                        PosibleCallers.Add(callerPlug.name, new List<string>());
                    PosibleCallers[callerPlug.name].Add(plug.Name);
                }   
            }
            else 
                throw new Exception(string.Format("The plugin {0} is LazyLoad in true, and does not have any callerPlugin Id", plug.Name));
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
            if(obj!= null && LoadedPluginsByType.ContainsKey(obj.GetType()))
            {
                Plugin plug = LoadedPluginsByType[obj.GetType()].Key;
                if(plug != null && plug.Outputs != null && plug.Outputs.Length > 0)
                    foreach (PluginOutput item in plug.Outputs)
                    {
                        System.Reflection.PropertyInfo prop = obj.GetType().GetProperty(item.GetterProperty);
                        object output = prop.GetValue(obj, null);
                        if(!Outputs.ContainsKey(output.GetType()))
                            Outputs.Add(output.GetType(), new List<KeyValuePair<string,object>>());
                        Outputs[output.GetType()].Add(new KeyValuePair<string,object>(plug.Name, output)); 
                    }
            }
        }

        /// <summary>
        /// Available outputs for that type.
        /// String: plugin name of the output
        /// object: the output
        /// </summary>
        /// <param name="ty"></param>
        /// <returns></returns>
        public List<KeyValuePair<string,object>> GetOutputsForType(Type ty) 
        {
            if (ty != null && Outputs.ContainsKey(ty))
                return Outputs[ty];
            return null;
        }
       

        public Plugin[] GetInitialPlugs()
        {
            return InitialPlugins.Values.ToArray<Plugin>();
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
            List<Plugin> plList;
            Plugin plg;
            if (LoadedPluginsByType.ContainsKey(ty))
                plg = LoadedPluginsByType[ty].Key;
            else
                return null;
            List<string> list = GetCalledPluginsBy(plg);
            if (list != null && list.Count >0)
            {
                plList = new List<Plugin>();
                foreach (string item in list)
                {
                    KeyValuePair<bool, Plugin> tm = this.SearchPlugin(item); //TODO: no llama 2 veces al plaguin si esta loadeado
                    if (!tm.Key && tm.Value != null)
                        plList.Add(tm.Value);
                }
                if (plList.Count > 0)
                    return plList;
            }
            return null;
        }

        /// <summary>
        /// returns all the plugins must be initiated called by 
        /// </summary>
        /// <param name="pl"></param>
        /// <returns></returns>
        private List<string> GetCalledPluginsBy(Plugin pl)
        {

            if (PosibleCallers.ContainsKey(pl.Name))
                return PosibleCallers[pl.Name];
            return null;

        }


        #region IDisposable Members

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

        
    //static class ExtensionMethods
    //{
    //    public static TSource[] ToArray<TSource>(this IEnumerable source)
    //    {
    //        return source.Cast<TSource>().ToArray();
    //    }
    //}
}
