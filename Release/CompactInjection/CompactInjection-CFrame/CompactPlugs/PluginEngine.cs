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

        private IPluginLocator _PluginLocator;
        public IPluginLocator PluginLocator 
        {
            private set 
            {   
                _PluginLocator = value;
                PluginLocator.NewPlugins += new EventHandler<NewPlugsEventArgs>(PluginLocator_NewPlugins);
            }
            get { return _PluginLocator;  }
        }
        private CompactPlugsRegistry PlugsRegistry { get; set; }
        private CompactConstructor Constructor { get; set; }

        #region Constructors and Init
        public PluginEngine(IPluginLocator locator)
        {
            PluginLocator = locator;
            
        }
        public PluginEngine()
        {

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
            if (PluginLocator == null)
                throw new Exception("PlugEngine needs a ILocator to work");
            Constructor = new CompactConstructor(PluginLocator.ObjectDefinitions, "default");
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
                if (!string.IsNullOrEmpty(plug.InstallMethod))
                {
                    Type ty = Type.GetType(plug.Type);
                    MethodInfo installMethod = ty.GetMethod(plug.InstallMethod);
                    installMethod.Invoke(Constructor.Create(ty), new object[] { });
                }
            }
            catch (Exception e) 
            {
                throw new Exception(string.Format("The plugin %1 couldn´t install", plug.Name), e);
            }    
        }

        private void RunPlugin(Plugin plug)
        {
           
            Type ty = GetTypeForFileName(plug.FileName, plug.Type);//Assembly.LoadFrom(plug.FileName).GetType(plug.Type);
            if (ty == null)
                throw new Exception(string.Format("The plugin {0} could not load the type. Are yo missing the Assembly Filename? ", plug.Name));
            try
            {    
                CheckAndRunDependencies(plug);
                object obj = Constructor.Create(ty);
                obj = InjectInputs(obj, plug);
                MethodInfo runMethod = ty.GetMethod(plug.RunMethod);
                runMethod.Invoke(obj, new object[] { });
                PlugsRegistry.AddLoadedPlugin(plug, obj);
                PlugsRegistry.ExtractOutputs(obj);
                string parent = obj.GetType().BaseType.ToString();
                if (parent == "System.Windows.Forms.Form" && WinForm == null)
                    this.WinForm = obj;
                
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("The plugin {0} couldn´t initialize", plug.Name), e);
            }
        }

        private static Type GetTypeForFileName(string filename, string stype)
        {
            Type tipo;
            if (!string.IsNullOrEmpty(filename))
                tipo = Assembly.LoadFrom(filename).GetType(stype);
            else
            {
               tipo = Type.GetType(stype, true);
            }
            return tipo;
        }

        private object InjectInputs(object obj, Plugin plug)
        {
            if (plug.Inputs != null && plug.Inputs.Length > 0)
            {
                ObjectDefinition objDef = CreateObjectDefinition(obj, plug);
                if (objDef != null)
                {
                    Constructor.AddDefinition(objDef, "default");
                    return Constructor.Build<object>(obj, objDef.Name);
                }
            }
            return obj;
        }

        private ObjectDefinition CreateObjectDefinition(object obj, Plugin plug)
        {
            if (plug != null && obj!= null && plug.Inputs != null && plug.Inputs.Length > 0)
            {
                ObjectDefinition objDef = new ObjectDefinition();
                objDef.Name = plug.Name + "Plug";
                objDef.Type = obj.GetType().ToString();
                List<Property> propertiesToInject = new List<Property>();
                foreach (PluginInput input in plug.Inputs)
                {
                    List<KeyValuePair<string, object>> outList = PlugsRegistry.GetOutputsForType(GetTypeForFileName(input.FileName, input.Type));
                    if (outList != null)
                    {
                        if (outList.Count == 1 || outList.Count > 1) //TODO: falta Desarrollar cuando hay mas de un plugin para el mismo tipo
                            propertiesToInject.Add(new Property(input.SetterProperty, outList[0].Value.GetType(), plug.FileName, outList[0].Value));

                    }
                }
                objDef.Properties = propertiesToInject.ToArray();
                return objDef; 
            }
            return null;
        }

        private void RunPlugin(string pluginName)
        {
            KeyValuePair<bool, Plugin> val = PlugsRegistry.SearchPlugin(pluginName);
            if (!val.Key) //chequea q el mismo plugin no corra mas de una vez. TODO: pensar esto
                RunPlugin(val.Value);
        }
        private void CheckAndRunDependencies(Plugin plug)
        {
            if (plug != null && plug.DependentPlugins != null && plug.DependentPlugins.Length > 0)
                foreach (DependentPlugin item in plug.DependentPlugins)
                {
                    if (!PlugsRegistry.IsPluginLoaded(item.name))
                        RunPlugin(item.name);
                }          
        }

        public Object WinForm { get; private set; }

    }


}
