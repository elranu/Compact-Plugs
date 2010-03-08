//Develop by Mariano Julio Vicario -
//http://compactplugs.codeplex.com/
//http://www.ranu.com.ar (Blog)
//Microsoft Public License (Ms-PL)
using System;
using System.Collections.Generic;
using System.Text;
using CompactInjection.ConfigurationObjects;

namespace CompactInjection
{
    public partial class CompactConstructor
    {
        private sealed class ObjectDefinitionRegistry
        {
            //Dicionario de <Contextos,<Nombres de object definitions, ObjectDefinitions>
            private Dictionary<string, Dictionary<string, ObjectDefinition>> _registry;

            public ObjectDefinitionRegistry()
            {
                _registry = new Dictionary<string, Dictionary<string, ObjectDefinition>>();
            }
            
            /// <summary>
            /// Adds an array Object Definition to the Registry
            /// </summary> 
            /// <param name="cont"></param>
            public void AddDefinitions(CompactContainer cont) {
                foreach (Context var in cont.Contexts)
                {
                    AddDefinitions(var);
                }
            }
            /// <summary>
            /// Adds an array Object Definition to the Registry
            /// </summary>
            /// <param name="context"></param>
            public void AddDefinitions(Context context) {
                foreach (ObjectDefinition var in context.ObjectDefinitions)
                {
                    AddDefinition(context.Name, var);   
                }
            }
            /// <summary>
            /// Adds a Object Definition to the Registry
            /// </summary>
            /// <param name="context">name of the context</param>
            /// <param name="obj"></param>
            public void AddDefinition(string context, ObjectDefinition obj){
                try
                {
                    if (!_registry.ContainsKey(context))
                        _registry.Add(context, new Dictionary<string, ObjectDefinition>());
                    if(!_registry[context].ContainsKey(obj.Name))
                        _registry[context].Add(obj.Name, obj);
                }
                catch (ArgumentException ex)
                {

                    throw new Exception("", ex);
                }
            }

            /// <summary>
            /// Search's the requested object definition
            /// </summary>
            /// <param name="context">Name of the context</param>
            /// <param name="name">Name of the Object Definition</param>
            /// <returns>null if objectDefinition is not found, else the corresponing OBJDefinition</returns>
            public ObjectDefinition GetDefinition(string context, string nameObjDefinition) {
                if (!_registry.ContainsKey(context))
                    return null;
                if (!_registry[context].ContainsKey(nameObjDefinition))
                    return null;
                return _registry[context][nameObjDefinition];
            }

        }
    }
}
