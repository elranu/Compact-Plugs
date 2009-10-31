using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompactPlugs_Primitives
{
    interface IPluginRegistry
    {

         void SearchPlugins();
         CompactPlugs Plugins { get;  }
    
    }
}
