using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using CompactInjection.Interfaces;
using CompactInjection.ConfigurationObjects;
using CompactInjection.Tools;

//Develop by Mariano Julio Vicario -
//http://compactplugs.codeplex.com/
//http://www.ranu.com.ar (Blog)
//Microsoft Public License (Ms-PL)



namespace CompactInjection
{
    public partial class CompactConstructor
    {
        private string _ContextName = string.Empty;
        private ObjectDefinitionRegistry _registry = new ObjectDefinitionRegistry();
        private SingletonRegistry _singleRegistry = new SingletonRegistry();
        private IContextSelector _contextSelector;

        #region Propeties
        public string ContextName
        {
            get { return _ContextName; }
            set { _ContextName = value; }
        }

        public IContextSelector ContextSelector
        {
            get { return _contextSelector; }
            set { _contextSelector = value; }
        } 
        #endregion

        #region Constructors
        /// <summary>
        /// Defines the current context in "default".
        /// </summary>
        public CompactConstructor()
        {
            _ContextName = "default";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">Actual context. Defines the current context</param>
        public CompactConstructor(string contextName)
        {
            _ContextName = contextName;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">Actual context. Defines the current context</param>
        /// <param name="xmlConfiguration">full path of a xmlConfiguration</param>
        public CompactConstructor(string contextName, string xmlConfiguration)
        {
            _ContextName = contextName;
            LoadConfiguration(xmlConfiguration);
        }

        public CompactConstructor(CompactContainer container, string contextName)
        {
            _ContextName = contextName;
            _registry.AddDefinitions(container);
        }

        public CompactConstructor(Context context, string contextNmae)
        {
            _ContextName = contextNmae;
            _registry.AddDefinitions(context);
        }

        #endregion

        #region DefaultConstructor - Singleton
        private static CompactConstructor _thisSingleton;

        public static CompactConstructor DefaultConstructor
        {
            get
            {
                if (_thisSingleton == null)
                    _thisSingleton = new CompactConstructor();
                return _thisSingleton;
            }
        }
        #endregion

        #region Add Object Definitions
        public void AddDefinitions(string xmlFile)
        {
            LoadConfiguration(xmlFile);
        }

        public void AddDefinitions(CompactContainer cont)
        {
            _registry.AddDefinitions(cont);
        }

        public void AddDefinitions(Context context)
        {
            _registry.AddDefinitions(context);
        }

        /// <summary>
        /// Add Object Definition in the specified context
        /// </summary>
        /// <param name="objDef"></param>
        /// <param name="contextName">Context name where to add the ObjectDefinition objDef</param>
        public void AddDefinition(ObjectDefinition objDef, string contextName)
        {
            _registry.AddDefinition(contextName, objDef);
        } 
        #endregion

        #region Private Methods
        private void LoadConfiguration(string xmlConfiguration)
        {
            _registry.AddDefinitions(XmlSerializerDeserializer.DeSerializer<CompactContainer>(xmlConfiguration));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="objToBeInjected"></param>
        /// <param name="propertieToBeInjected"></param>
        /// <param name="valueToInject"></param>
        /// <returns></returns>
        private static T SetProperty<T>(T objToBeInjected, string propertieToBeInjected, object valueToInject)
        {
            try
            {
                PropertyInfo pro = objToBeInjected.GetType().GetProperty(propertieToBeInjected, (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
                if(pro.PropertyType.IsInterface || pro.PropertyType.IsClass)
                    pro.SetValue(objToBeInjected, valueToInject, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, null);
                else
                    pro.SetValue(objToBeInjected, Convert.ChangeType(valueToInject, pro.PropertyType, null), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, null);
                return objToBeInjected;
            }
            catch (Exception e)
            {
                throw new Exception("Error al intentar setar la propiedad: " + propertieToBeInjected + " del objeto: " + objToBeInjected + " con el objeto del tipo: " + objToBeInjected.GetType().ToString(), e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static object NewObject(Type ty) 
        {
            try
            {
                ConstructorInfo con = ty.GetConstructor((BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly), null, new Type[] { }, null);
                return con.Invoke(new object[] { });
            }
            catch (Exception e)
            {
                throw new Exception("Error al intentar crear el objeto del Tipo: " + ty.ToString(), e);
            }
        }

        /// <summary>
        /// no considera los object en Singleton
        /// Por ahora solo se encarga de setear las propiedades.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objdef"></param>
        /// <param name="objToInject"></param>
        /// <returns>Injected Object</returns>
        private T InjectObject<T>(ObjectDefinition objdef, T objToInject) where T : class
        {
            T injectedObj = SetProperties<T>(objdef, objToInject);
            return injectedObj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objdef"></param>
        /// <param name="objToInject"></param>
        /// <returns>Object with the injected properties</returns>
        private T SetProperties<T>(ObjectDefinition objdef, T objToInject) where T : class
        {
            foreach (Property var in objdef.Properties)
            {
                if (!string.IsNullOrEmpty(var.Set))
                {
                    objToInject = SimpleSet<T>(objToInject, var);
                }
                else if (!string.IsNullOrEmpty(var.SetWithNewType))
                {
                    objToInject = SetWithNewType<T>(objToInject, var);
                }
                else if (!string.IsNullOrEmpty(var.SetWithObjectDefinition))
                {
                    objToInject = SimpleSetWihObjectDefinition<T>(objToInject, var);
                }
                else if (!string.IsNullOrEmpty(var.SetList))
                {
                    objToInject = SetList<T>(objToInject, var);
                }
                else if (!string.IsNullOrEmpty(var.SetDictionary))
                {
                    objToInject = SetDictionary<T>(objToInject, var);
                }
            }
            return objToInject;
        }

        /// <summary>
        /// El T y el Type ty  pueden ser distintos tipos
        /// </summary>
        /// <typeparam name="T">Type que se desea de vuelta</typeparam>
        /// <param name="objName"></param>
        /// <param name="ty">Type real para construir</param>
        /// <returns></returns>
        private T New<T>(string objName, Type ty) where T : class
        {
            if (_contextSelector != null)
                _ContextName = _contextSelector.GetCurrentContext();
            if (_singleRegistry.GetObject(ty) != null)
                return _singleRegistry.GetObject(ty) as T;
            ObjectDefinition objdef = _registry.GetDefinition(ContextName, objName);
            if (objdef != null)
            {
                T objToBeInjected = NewObject(ty) as T;
                T injectedObject = InjectObject<T>(objdef, objToBeInjected);
                if (objdef.Singleton)
                    _singleRegistry.AddSingleton(injectedObject);
                return injectedObject as T;
            }
            else
                throw new Exception("The type:" + ty.ToString() + " dosen´t has a ObjectDefinition");

        }

        #region Setter's Injectors
        /// <summary>
        /// Dosen't do anything at the moment
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objToInject"></param>
        /// <param name="var"></param>
        /// <returns></returns>
        private static T SetDictionary<T>(T objToInject, Property var)
        {
            return objToInject;
        }

        /// <summary>
        /// Dosen't do anything at the moment
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objToInject"></param>
        /// <param name="var"></param>
        /// <returns></returns>
        private static T SetList<T>(T objToInject, Property var)
        {
            return objToInject;
        }

        private T SimpleSetWihObjectDefinition<T>(T objToInject, Property var) where T : class
        {
            ObjectDefinition def = _registry.GetDefinition(_ContextName, var.SetWithObjectDefinition);
            Type tipo = Assembly.LoadFrom(def.FileName).GetType(def.Type);
            return SetProperty<T>(objToInject, var.Name, New<object>(var.SetWithObjectDefinition, tipo));
        }

        private static T SetWithNewType<T>(T objToInject, Property var)
        {
            Type tipo = Assembly.LoadFrom(var.FileName).GetType(var.SetWithNewType);
            return SetProperty<T>(objToInject, var.Name, NewObject(tipo));
        }

        private static T SimpleSet<T>(T objToInject, Property var)
        {
            return SetProperty<T>(objToInject, var.Name, var.Set);
        }
        #endregion
        
        #endregion

        #region NEW's
        /// <summary>
        /// New Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objName"></param>
        /// <returns></returns>
        public T New<T>(string objName) where T : class
        {
            if (_contextSelector != null)
                _ContextName = _contextSelector.GetCurrentContext();
            if (_singleRegistry.GetObject<T>() != null)
                return _singleRegistry.GetObject<T>();
            ObjectDefinition objdef = _registry.GetDefinition(_ContextName, objName);
            if (objdef != null)
            {
                T objToBeInjected = NewObject(typeof(T)) as T;
                T injectedObject = InjectObject<T>(objdef, objToBeInjected);
                if (objdef.Singleton)
                    _singleRegistry.AddSingleton<T>(injectedObject);
                return injectedObject;
            }
            else
                throw new Exception("The type:" + typeof(T).ToString() + " dosen´t has a ObjectDefinition");
        }

        /// <summary>
        /// New Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objName"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public T New<T>(string objName, string contextName) where T : class
        {
            _ContextName = contextName;
            return New<T>(objName);
        } 
        #endregion

        



        
        
    }
}
