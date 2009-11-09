using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompactInjection.ConfigurationObjects;

namespace CompactPlugs_Primitives
{
    public interface IPluginLocator
    {

         void SearchPlugins();
         PlugsContainer Plugins { get;  }
         CompactContainer ObjectDefinitions { get; }
    
    }
}
