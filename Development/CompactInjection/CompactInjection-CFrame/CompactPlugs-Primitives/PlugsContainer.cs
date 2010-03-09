using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace CompactPlugs_Primitives
{
    public partial class PlugsContainer 
    {
    
         
    
    }

    public class NewPlugsEventArgs : EventArgs
    {
        private PlugsContainer plugsContainer;

        public NewPlugsEventArgs(PlugsContainer _plugs)
        {
            plugsContainer = _plugs;
        }
        public PlugsContainer NewPlugins
        {
            get { return plugsContainer; }
            set { plugsContainer = value; }
        }
    }


}
