using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using CompactPlugs_Primitives;
using CompactInjection;
using System.Reflection;


namespace CompactPlugs
{
    class PluginEngine
    {

        public IPluginLocator PluginLocator { get; set; }
        public CompactPlugsRegistry PlugsRegistry { get; private set; }
        private CompactConstructor Constructor { get; set; }


        public PluginEngine()
        {
            
        }

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
            PlugsRegistry = new CompactPlugsRegistry();
            Constructor = new CompactConstructor(PluginLocator.ObjectDefinitions);
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
                object obj = Constructor.New(ty);
                MethodInfo runMethod = ty.GetMethod(plug.RunMethod);
                runMethod.Invoke(obj, new object[] { });//Todo chequear dependencias si estan cumplidas correrplugin
                PlugsRegistry.AddLoadedPlugin(plug, obj);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("The plugin %1 couldn´t initialize", plug.Name), e);
            }
        }
        

    }
}
