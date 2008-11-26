using System;
using System.Collections.Generic;
using System.Text;

namespace CompactContainer
{
    public partial class CompactConstructor
    {
        private class SingletonRegistry
        {
            private Dictionary<Type, object> _registry = new Dictionary<Type, object>();
            public SingletonRegistry()
            {

            }
            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns>devuelve null si no lo tiene</returns>
            public T GetObject<T>(){
               if(!_registry.ContainsKey(typeof(T)))
                   return default(T);
                return (T)_registry[typeof(T)];
            }

            public object GetObject(Type ty) {
                if (!_registry.ContainsKey(ty))
                    return null;
                return _registry[ty];
            }

            public void AddSingleton<T>(T obj) {
                _registry.Add(typeof(T), obj);
            }

            public void AddSingleton(object obj, Type ty) {
                _registry.Add(ty, obj);
            }
        }
    }
}
