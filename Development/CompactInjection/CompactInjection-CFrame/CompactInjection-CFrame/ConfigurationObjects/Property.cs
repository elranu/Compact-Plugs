using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace CompactInjection.ConfigurationObjects
{
    public partial class Property
    {

        public object ObjectToInject { get; set; }

        public Property(string name, Type ty, Object objectToInject)
        {
            this.Name = name;
            this.SetWithNewType = ty.ToString();
            this.ObjectToInject = objectToInject;
        }
        public Property()
        {

        }

      
    
    }
}
