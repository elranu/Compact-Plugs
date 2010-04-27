using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using CompactInjection.ConfigurationObjects;
using CompactInjection.Attributes;
using System.Reflection;

namespace CompactInjection.Tools
{
    /// <summary>
    /// This class will be responsable that form an object with CI attributes it will create an ODefinition. 
    /// So CI will support object definitions by XML, objects, and attributes
    /// TODO: on next version
    /// </summary>
    internal class ObjectDefinitionBuilder
    {

        public ObjectDefinitionBuilder()
        {

        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns>null if does not has Definition</returns>
        public ObjectDefinition GetObjectDefinition(Type ty) 
        {
            Object[] classAt = ty.GetCustomAttributes(typeof(InjectableAttribute), false);
            if (classAt.Length > 0)
            {
                ObjectDefinition objDef = new ObjectDefinition();
                objDef.Name = ty.ToString();
                objDef.Type = ty.ToString();
                objDef.Singleton = ((InjectableAttribute)classAt[0]).Singleton;
                objDef.FileName = ty.Assembly.GetName().Name;
                objDef.Properties = this.BuildProperties( ty.GetProperties());
            }
            return null;
        }

        private Property[] BuildProperties(PropertyInfo[] propsInfo)
        {
            List<Property> props = new List<Property>();
            if (propsInfo != null && propsInfo.Length > 0) 
            {
                foreach (PropertyInfo item in propsInfo)
                {
                    InjectAttribute inj = this.GetInjectAttribute(item);
                    Property prop = new Property(item.Name);
                    if (!item.PropertyType.IsInterface && !item.PropertyType.IsClass)
                    {
                        prop.Set = inj.ValueToInject as string;
                    }
                    else
                    {
                        FillPropIfIsRefType(item, inj, prop);
                    }
                    props.Add(prop);
                }
            }
            return props.ToArray();
        }

        private static void FillPropIfIsRefType(PropertyInfo item, InjectAttribute inj, Property prop)
        {
            if (item.PropertyType.IsGenericType)
            {
                if (item.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    prop.SetList = inj.ValueToInject as string;
                    prop.ListType = item.PropertyType.GetGenericArguments()[0].GetType().ToString();
                }
                else
                {
                    prop.SetDictionary = inj.ValueToInject as string;
                    prop.KeyType = item.PropertyType.GetGenericArguments()[0].GetType().ToString();
                    prop.ValueType = item.PropertyType.GetGenericArguments()[1].GetType().ToString();
                }

            }
            else
            {
                if (inj.IsObjectDefinition)
                    prop.SetWithObjectDefinition = inj.ValueToInject as string;
                else // is a setWithNewType
                {
                    prop.SetWithNewType = inj.ValueToInject as string;
                    prop.FileName = inj.ValueToInject.GetType().Assembly.FullName;
                }

            }
        }

        private InjectAttribute GetInjectAttribute(PropertyInfo info)
        {
            object[] att = info.GetCustomAttributes(typeof(InjectAttribute), false);
            if (att.Length > 0) {
                return att[0] as InjectAttribute;
            }
            return null;
        }
    
    }
}
