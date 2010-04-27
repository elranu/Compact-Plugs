using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using CompactInjection.ConfigurationObjects;

namespace CompactInjection.Attributes
{
    [global::System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class InjectableAttribute : Attribute
    {

        
        public bool Singleton { get; set; }
        public InjectableAttribute()
        {
            Singleton = false;
        }
       
        
    }

    [global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class InjectAttribute : Attribute
    {


        public object ValueToInject { get; private set; }
        public bool IsObjectDefinition { get; set; }
        public InjectAttribute(object valueToInject)
        {
            ValueToInject = valueToInject;
            IsObjectDefinition = false;
        }

    }
}
