using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using CompactInjection;
using CompactInjection.ConfigurationObjects;
using CompactPlugs_Primitives;
using CompactPlugs;
namespace TestCompactPlugs1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            ObjectDefinition objDef = new ObjectDefinition();
            objDef.Name = "PluginEngine";
            objDef.Type = typeof(CompactPlugs.PluginEngine).ToString();
            objDef.FileName = "CompactPlugs.dll";
            objDef.Singleton = true;
            Property p = new Property("PluginLocator", typeof(CompactPlugs.PluginLocators.SimpleFilePluginLocator), "CompactPlugs.dll");
            objDef.Properties = new Property[] { p };
            CompactInjection.CompactConstructor.DefaultConstructor.AddDefinition(objDef, "default");
            PluginEngine plugEngine = CompactConstructor.DefaultConstructor.New<PluginEngine>("PluginEngine");

            plugEngine.Run();
            Application.Run(plugEngine.WinForm as System.Windows.Forms.Form);
        }
    }
}