using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using CompactPlugs_Primitives;
using CompactInjection;
using CompactInjection.ConfigurationObjects;
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
            PluginLocator.NewPlugins += new EventHandler<NewPlugsEventArgs>(PluginLocator_NewPlugins);
        }

        private void PluginLocator_NewPlugins(object sender, NewPlugsEventArgs e)
        {
            PlugsRegistry.Add(e.NewPlugins);
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

        public List<Plugin> SearchPluginsByCategory(string category)
        {
            return PlugsRegistry.SearchPluginsByCategory(category);
        }

        public void CallPlugins(object obj)
        {
            PlugsRegistry.ExtractOutputs(obj);
            List<Plugin> plugs = PlugsRegistry.GetCalledPlugins(obj.GetType());
            if(plugs != null && plugs.Count >0)
                foreach (Plugin item in plugs)
                {
                    RunPlugin(item);
                }
        }
        public void CallPlugins(object obj, string str)
        {
            throw new  NotImplementedException();
        }

        public void UnloadPlugin(object obj)
        {
            throw new NotImplementedException();   
        }

        private void InstallPlugin(Plugin plug)
        {
            try
            {
                Type ty = Type.GetType(plug.Type);
                MethodInfo installMethod = ty.GetMethod(plug.InstallMethod);
                installMethod.Invoke(Constructor.Create(ty), new object[] { });
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
                object obj = Constructor.Create(ty);
                if(plug.LazyLoad)//TODO: bug: si los initial plugs tiene inputs de otros initial plugs no anda
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
            ObjectDefinition objDef = CreateObjectDefinition(obj, plug);
            Constructor.AddDefinition(objDef, "default");
            return Constructor.Build<object>(obj, objDef.Name);
        }

        private ObjectDefinition CreateObjectDefinition(object obj, Plugin plug)
        {
            ObjectDefinition objDef = new ObjectDefinition();
            objDef.Name = plug.Name + "Plug";
            objDef.Type = obj.GetType().ToString();
            List<Property> propertiesToInject = new List<Property>();
            foreach (PluginInput input in plug.Inputs)
            {
                List<KeyValuePair<string, object>> outList = PlugsRegistry.GetOutputsForType(Type.GetType(input.Type));
                if (outList == null)
                {
                    if (outList.Count == 1 || outList.Count > 1) //TODO: falta Desarrollar cuando hay mas de un plugin para el mismo tipo
                        propertiesToInject.Add( new Property(input.SetterProperty, outList[0].Value.GetType(), outList[0].Value));
                    
                }
            }
            objDef.Properties = propertiesToInject.ToArray();
            return objDef;
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
