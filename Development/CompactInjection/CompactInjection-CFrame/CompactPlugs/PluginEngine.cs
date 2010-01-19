using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using CompactPlugs_Primitives;
using CompactInjection;
using System.Reflection;

//Develop by Mariano Julio Vicario -
//http://compactplugs.codeplex.com/
//http://www.ranu.com.ar (Blog)
//Microsoft Public License (Ms-PL)
namespace CompactPlugs
{
    public class PluginEngine
    {

        public IPluginLocator PluginLocator { get; set; }
        private CompactPlugsRegistry PlugsRegistry { get; set; }
        private CompactConstructor Constructor { get; set; }

        #region Constructors and Init
        public PluginEngine(IPluginLocator locator)
        {
            PluginLocator = locator;
        }

        public void Run()
        {
            Init();
            RunInitialPlugs();
        }

        private void Init()
        {
            Constructor = new CompactConstructor(PluginLocator.ObjectDefinitions);
            PlugsRegistry = new CompactPlugsRegistry(Constructor);
            PluginLocator.SearchPlugins();
            PlugsRegistry.Add(PluginLocator.Plugins);
        }

        private void RunInitialPlugs()
        {
            foreach (Plugin item in PlugsRegistry.GetInitialPlugs())
            {
                if (!PlugsRegistry.IsPluginInstalled(item))
                    InstallPlugin(item);
                RunPlugin(item);
            }
        }
        #endregion

        public void CallPlugins(object obj)
        {
            PlugsRegistry.ExtractOutputs(obj);
            List<Plugin> plugs = PlugsRegistry.GetCalledPlugins(obj.GetType());
        }
        public void CallPlugins(object obj, string str)
        {
            
        }

        public void UnloadPlugin(object obj)
        {
            //PlugsRegistry.   
        }

        private void InstallPlugin(Plugin plug)
        {
            try
            {
                Type ty = Type.GetType(plug.Type);
                MethodInfo installMethod = ty.GetMethod(plug.InstallMethod);
                installMethod.Invoke(Constructor.New(ty), new object[] { });
            }
            catch (Exception e) 
            {
                throw new Exception(string.Format("The plugin %1 couldn´t install", plug.Name), e);
            }    
        }

        private void RunPlugin(Plugin plug)
        {
            try
            {
                Type ty = Type.GetType(plug.Type);
                CheckAndRunDependencies(plug);
                object obj = Constructor.New(ty);
                //TODO:falta injectar los outputs
                obj = InjectInputs(obj, plug);
                MethodInfo runMethod = ty.GetMethod(plug.RunMethod);
                runMethod.Invoke(obj, new object[] { });
                PlugsRegistry.AddLoadedPlugin(plug, obj);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("The plugin %1 couldn´t initialize", plug.Name), e);
            }
        }

        private object InjectInputs(object obj, Plugin plug)
        {
            throw new Exception("falta, todo");
        }

        private void RunPlugin(string pluginName)
        {
            KeyValuePair<bool, Plugin> val = PlugsRegistry.SearchPlugin(pluginName);
            if (!val.Key) //chequea q el mismo plugin no corra mas de una vez. TODO: pensar esto
                RunPlugin(val.Value);
        }
        private void CheckAndRunDependencies(Plugin plug)
        {
            if(plug.DependentPlugins.Length > 0)
                foreach (DependentPlugin item in plug.DependentPlugins)
                {
                    if (!PlugsRegistry.IsPluginLoaded(item.name))
                        RunPlugin(item.name);
                }        
        }        

    }


}
