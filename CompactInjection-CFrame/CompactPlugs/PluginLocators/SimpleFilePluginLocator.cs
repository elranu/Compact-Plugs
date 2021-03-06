﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using CompactPlugs_Primitives;
using System.IO;
using CompactInjection.Tools;
using System.Reflection;

namespace CompactPlugs.PluginLocators
{
    public class SimpleFilePluginLocator : IPluginLocator
    {
        string fileName = "plugins.txt";
        CompactPlugs_Primitives.PlugsContainer newCont = new PlugsContainer();

        #region IPluginLocator Members
        /// <summary>
        /// Just reads the file plugins.txt in the executing directory
        /// Per line the name of a PluginContainer xml 
        /// </summary>
        public void SearchPlugins()
        {
            StreamReader SR;
            string S;
            List<Plugin> listCont = new List<Plugin>();
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            SR = File.OpenText(System.IO.Path.Combine( dir, fileName));
            do
            {
                S = SR.ReadLine();
                if (!string.IsNullOrEmpty(S))
                {
                    S = System.IO.Path.Combine(dir, S);
                    CompactPlugs_Primitives.PlugsContainer cont = XmlSerializerDeserializer.DeSerializer<CompactPlugs_Primitives.PlugsContainer>(S);
                    listCont.AddRange(cont.Plugins);
                }
            } while (S != null);
            SR.Close();

            newCont.Plugins = listCont.ToArray();

        }

        public PlugsContainer Plugins
        {
            get { return newCont; }
        }

        public CompactInjection.ConfigurationObjects.CompactContainer ObjectDefinitions
        {
            get { return null; }
        }

        public event EventHandler<NewPlugsEventArgs> NewPlugins;

        #endregion
    }
}
