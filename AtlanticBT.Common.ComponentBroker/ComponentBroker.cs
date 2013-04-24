using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace AtlanticBT.Common.ComponentBroker
{
    /// <summary>
    /// This is an implementation of a Dependency Lookup.
    /// The component broker is used as a single point of object instantiation.
    /// The purpose of this class is to aid in unit testing by making 
    /// Depended on Components (DOCs) easier to replace.
    /// It also aids in sharing resources rather than instantiating multiple instances of them.
    /// This is done as an alternative to Dependency Injection.
    /// 
    /// The component broker retrieves components through 3 primary methods. 
    /// The most common is by utilizing a naming convention where interfaces can be 
    /// mapped to their concrete implementation. Alternate methods allow for a factory 
    /// or explicit type to be associated with a type/key. If a factory is specified, 
    /// then it will be instantiated and used to create the instance. If a type is 
    /// specified then that type will be instantiated.
    /// 
    /// The components live in HttpContext.Current.Items which has a lifespan of the HTTP request.
    /// This means that any registered component will be disposed of at the end of every server hop 
    /// from the client. The factory and type mappings are stored in static dictionaries that allow 
    /// them to persist for the life of the application.
    /// </summary>
    public static class ComponentBrokerInstance
    {
        #region INTERFACE CONVENTION

        /// <summary>
        /// Defines the convention used to map an interface to their implementation.
        /// The regex should define what should be removed from the name of an interface to
        /// get the implementation name.
        /// The default convention is that an interface has the same name as its
        /// implementation but it starts with the letter 'I'.
        /// </summary>
        private const string Standardinterfacemask = "^I";
        private static String _interfaceMask = Standardinterfacemask;
        public static String InterfaceRegexMask
        {
            get { return _interfaceMask; }
            set { _interfaceMask = value; }
        }

        #endregion

        #region FACTORY CACHE

        /// <summary>
        /// Component factory storage.
        /// For components that require advanced setup, such as parameterized constructors, 
        /// the developer can register a factory to create it.
        /// </summary>
        private static Dictionary<string, string> Factories { get; set; }

        #endregion
        
        #region TYPE ASSOCIATION CACHE

        /// <summary>
        /// Component type association storage.
        /// For components that don't follow the default interface convention 
        /// or for instances where there are multiple implementations of an interface,
        /// a type to type association can be created.
        /// This is different than registering a component in that the object is only
        /// created when it is needed.
        /// </summary>
        private static Dictionary<string, string> TypeAssociations { get; set; }

        #endregion

        #region COMPONENT CACHE

        /// <summary>
        /// Component storage.
        /// Most componenets are registered to their fully qualified name.
        /// They can also be registered to the fully qualified name of their interface 
        /// or even to an arbitrary string which allows multiple registrations of the same
        /// component type.
        /// 
        /// The cache uses HttpContext.Current.Items when running on a website so that 
        /// the components are not shared between users and only live for one hop. 
        /// There is a static fallback for testing.
        /// </summary>
        private const String ComponentCacheKey = "ABT_ComponentInstances";
        private static Dictionary<string, object> Components
        {
            get
            {
                //if not running tests use the http context to avoid static issues
                if (HttpContext.Current != null)
                {
                    var items = HttpContext.Current.Items;
                    if (!items.Contains(ComponentCacheKey))
                    {
                        items[ComponentCacheKey] = new Dictionary<string, object>();
                    }
                    return items[ComponentCacheKey] as Dictionary<string, object>;
                }

                //else use the static variable since HTTPContext is not available
                return _components;
            }
            set 
            {
                //if not running tests use the http context to avoid static issues
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Items[ComponentCacheKey] = value; 
                }

                //else use the static variable since HTTPContext is not available
                _components = value;
            }
        } 
        private static Dictionary<string, object> _components = new Dictionary<string, object>();

        #endregion

        /// <summary>
        /// Initialize the cache.
        /// </summary>
        static ComponentBrokerInstance()
        {
            Factories = new Dictionary<string, string>();
            TypeAssociations = new Dictionary<string, string>();
        }

        #region FACTORIES

        #region REGISTER COMPONENT FACTORIES

        /// <summary>
        /// Register a component factory for the given type.
        /// </summary>
        /// <typeparam name="TComponent">Type of the component to register</typeparam>
        /// <typeparam name="TFactory">Type of the factory to register</typeparam>
        public static void RegisterFactory<TComponent, TFactory>() where TFactory : IComponentFactory<TComponent>
        {
            RegisterFactory<TFactory>(typeof(TComponent).AssemblyQualifiedName);
        }

        /// <summary>
        /// Register a component factory for the given key.
        /// </summary>
        /// <typeparam name="TFactory">Type of the factory to register</typeparam>
        /// <param name="key">Key to register the component to</param>
        /// <exception cref="ArgumentException">Key must not be null or empty.</exception>
        public static void RegisterFactory<TFactory>(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key must not be null or empty.", "key");

            Factories[key] = typeof(TFactory).AssemblyQualifiedName;
        }

        #endregion

        #region HAS FACTORY

        /// <summary>
        /// Is a factory registered for the given component type?
        /// </summary>
        /// <typeparam name="TComponent">Type of the component</typeparam>
        /// <returns></returns>
        public static bool HasFactory<TComponent>()
        {
            return HasFactory(typeof(TComponent).AssemblyQualifiedName);
        }

        /// <summary>
        /// Is a factory registered for the given key?
        /// </summary>
        /// <param name="key">Key the factory is registered to</param>
        /// <exception cref="ArgumentException">Key must not be null or empty.</exception>
        /// <returns>Is the factory registered</returns>
        public static bool HasFactory(String key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key must not be null or empty.", "key");

            return Factories.ContainsKey(key);
        }

        #endregion

        #region UNREGISTER FACTORY

        /// <summary>
        /// Unregister a specific factory that is registered for the given component type.
        /// </summary>
        /// <typeparam name="TComponent">Type of the component the factory is registered to</typeparam>
        public static void UnregisterFactory<TComponent>()
        {
            UnregisterFactory(typeof(TComponent).AssemblyQualifiedName);
        }

        /// <summary>
        /// Unregister a specific factory that is registered for the given key.
        /// </summary>
        /// <param name="key">Key the factory is registered for</param>
        /// <exception cref="ArgumentException">Key must not be null or empty.</exception>
        public static void UnregisterFactory(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key must not be null or empty.", "key");

            if (HasFactory(key))
            {
                Factories.Remove(key);
            }
        }

        /// <summary>
        /// Unregister all factories.
        /// </summary>
        public static void UnregisterAllFactories()
        {
            Factories.Clear();
        }

        #endregion

        #endregion

        #region TYPE ASSOCIATIONS

        #region REGISTER TYPE ASSOCIATIONS

        /// <summary>
        /// Register a concrete type for the given type.
        /// </summary>
        /// <typeparam name="TComponent">Type of the interface to register</typeparam>
        /// <typeparam name="TConcrete">Type of the concrete type to register</typeparam>
        public static void RegisterType<TComponent, TConcrete>()
        {
            RegisterType<TConcrete>(typeof(TComponent).AssemblyQualifiedName);
        }

        /// <summary>
        /// Register a concrete type for the given key.
        /// </summary>
        /// <typeparam name="TConcrete">Type of the concrete type to register</typeparam>
        /// <param name="key">Key to register the component to</param>
        /// <exception cref="ArgumentException">Key must not be null or empty.</exception>
        public static void RegisterType<TConcrete>(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key must not be null or empty.", "key");

            TypeAssociations[key] = typeof(TConcrete).AssemblyQualifiedName;
        }

        #endregion

        #region HAS TYPE ASSOCIATION

        /// <summary>
        /// Is a concrete type registered for the given component type?
        /// </summary>
        /// <typeparam name="TComponent">Type of the component</typeparam>
        /// <returns></returns>
        public static bool HasType<TComponent>()
        {
            return HasType(typeof(TComponent).AssemblyQualifiedName);
        }

        /// <summary>
        /// Is a concrete type registered for the given key?
        /// </summary>
        /// <param name="key">Key the type is registered to</param>
        /// <exception cref="ArgumentException">Key must not be null or empty.</exception>
        /// <returns>Is the type registered?</returns>
        public static bool HasType(String key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key must not be null or empty.", "key");

            return TypeAssociations.ContainsKey(key);
        }

        #endregion

        #region UNREGISTER TYPE ASSOCIATION

        /// <summary>
        /// Unregister a specific concrete type that is registered for the given component type.
        /// </summary>
        /// <typeparam name="TComponent">Type of the component the concrete type is registered to</typeparam>
        public static void UnregisterType<TComponent>()
        {
            UnregisterType(typeof(TComponent).AssemblyQualifiedName);
        }

        /// <summary>
        /// Unregister a specific concrete type that is registered for the given key.
        /// </summary>
        /// <param name="key">Key the concrete type is registered for</param>
        /// <exception cref="ArgumentException">Key must not be null or empty.</exception>
        public static void UnregisterType(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key must not be null or empty.", "key");

            if (HasType(key))
            {
                TypeAssociations.Remove(key);
            }
        }

        /// <summary>
        /// Unregister all concrete types.
        /// </summary>
        public static void UnregisterAllTypes()
        {
            TypeAssociations.Clear();
        }

        #endregion

        #endregion

        #region COMPONENTS

        #region REGISTER COMPONENT

        /// <summary>
        /// Register a specific component for the given type.
        /// Will overwrite any previous registration for the same type.
        /// Used to register an existing component for a type other than its native type.
        /// Generally used to register a component to its interface.
        /// </summary>
        /// <typeparam name="TComponent">Type of the component to register</typeparam>
        /// <param name="component">Component to register</param>
        /// <exception cref="ArgumentException">Component must not be null.</exception>
        public static void RegisterComponent<TComponent>(object component)
        {
            RegisterComponent(typeof(TComponent).AssemblyQualifiedName, component);
        }

        /// <summary>
        /// Register a specific component for a given string.
        /// Will overwrite any previous registration for the same type.
        /// Used when their is a need to register multiple instances of the same component.
        /// </summary>
        /// <param name="key">Key to register the component to</param>
        /// <param name="component">Component to register</param>
        /// <exception cref="ArgumentException">Key must not be null or empty.</exception>
        /// <exception cref="ArgumentException">Component must not be null.</exception>
        public static void RegisterComponent(string key, object component)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key must not be null or empty.", "key");
            if (component == null) throw new ArgumentException("Component must not be null.", "component");

            Components[key] = component;
        }

        #endregion REGISTER COMPONENT

        #region HAS COMPONENT

        /// <summary>
        /// Is a component registered for a given type?
        /// Used when the desired type is known and can be 
        /// passed as a typeparam.
        /// </summary>
        /// <typeparam name="TComponent">Type of component</typeparam>
        /// <returns></returns>
        public static bool HasComponent<TComponent>()
        {
            return HasComponent(typeof(TComponent).AssemblyQualifiedName);
        }

        /// <summary>
        /// Is a component registerd for a given key?
        /// Used when a component is not registered to its 
        /// type's fully qualified name.
        /// </summary>
        /// <param name="key">Key for component</param>
        /// <exception cref="ArgumentException">Key must not be null or empty.</exception>
        /// <returns></returns>
        public static bool HasComponent(String key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key must not be null or empty.", "key");

            return Components.ContainsKey(key);
        }

        #endregion HAS COMPONENT

        #region RETRIEVE COMPONENT
        
        /// <summary>
        /// Retrieve a new instance of the component for the given type.
        /// This will create a new instance of the same component or register 
        /// it for the first time.
        /// </summary>
        /// <typeparam name="TComponent">Type of component</typeparam>
        /// <param name="register">Register the component once it has been retrieved?</param>
        /// <param name="args">Constructor parameters for the component to be initialized (or for the factory if one is registered)</param>
        /// <exception cref="ComponentBrokerException">If the component could not be created.</exception>
        /// <returns>The registered component</returns>
        public static TComponent RetrieveNewComponent<TComponent>(bool register = true, params object[] args)
        {
            //if it exists then reinstantiate it
            if (HasComponent<TComponent>())
            {
                var component = CreateComponent<TComponent>(args);

                // if the component should be registered then register it
                if (register)
                {
                    RegisterComponent<TComponent>(component);
                }

                return component;
            }

            return RetrieveComponent<TComponent>(register, args);
        }

        /// <summary>
        /// Retrieve the registered instance of the component for the given type.
        /// Create and register an instance of the component 
        /// if there is no regestered component for the provided type.
        /// </summary>
        /// <typeparam name="TComponent">Type of component</typeparam>
        /// <param name="register">Register the component once it has been retrieved?</param>
        /// <param name="args">Constructor parameters for the component to be initialized (or for the factory if one is registered)</param>
        /// <exception cref="ComponentBrokerException">If the component could not be created.</exception>
        /// <returns>The registered component</returns>
        public static TComponent RetrieveComponent<TComponent>(bool register = true, params object[] args)
        {
            // if the component is is registered then retrieve and return it
            if (HasComponent<TComponent>())
            {
                return RetrieveComponent<TComponent>(typeof (TComponent).AssemblyQualifiedName);
            }

            // since the component wasn't registered create a new component
            var component = CreateComponent<TComponent>(args);

            // if it should be registered then register it
            if (register)
            {
                RegisterComponent<TComponent>(component);
            }

            return component;
        }

        /// <summary>
        /// Create an instance of a component for the given type.
        /// If a factory exists for it then use the factory to create it.
        /// If a factory does not exists but a type association does, then create 
        /// an instance of that type. If neither exist, then use the nameing convention 
        /// to find the concrete implementation and create it. If it cannot be found 
        /// then return the default for the type.
        /// </summary>
        /// <typeparam name="TComponent">Type of component to create</typeparam>
        /// <param name="args">Constructor parameters for the component to be initialized</param>
        /// <exception cref="ComponentBrokerException">If the component could not be created.</exception>
        /// <returns>The component or the default for the type</returns>
        private static TComponent CreateComponent<TComponent>(params object[] args)
        {
            var type = typeof(TComponent);

            //If the component is an interface then find its implementation.
            if (type.IsInterface)
            {
                var key = type.AssemblyQualifiedName;

                if (key != null)
                {
                    // if a factory exists then use it
                    if (HasFactory(key))
                    {
                        // get the factory type
                        var factoryType = Factories[key];

                        // get an instance of the factory
                        var factory = CreateInstance(factoryType, args);

                        // if the factory exists then create an instance of the component
                        if (factory != null && factory is IComponentFactory<TComponent>)
                        {
                            return ((IComponentFactory<TComponent>)factory).Create();
                        }
                    }
                    // if the factory does not exist but a type exists then use it
                    else if (HasType(key))
                    {
                        // get the type
                        var componentType = TypeAssociations[key];

                        // get the component
                        return (TComponent)CreateInstance(componentType, args);
                    }
                    // if a factory and type association do not exist then use the naming convention
                    else
                    {
                        // generate the fully qualified name space of the component by convention
                        var maskedName = Regex.Replace(type.Name, InterfaceRegexMask, String.Empty);
                        var componentType = key.Replace(type.Name, maskedName);

                        // get the component
                        return (TComponent)CreateInstance(componentType, args);
                    }
                }
            }
            // if the component is an abstract class then there is no good way to find
            // the specific implementation that the user wants, so we wont create one.
            else if (!type.IsAbstract)
            {
                // create an instance of a concrete class since the type isn't an interface or abstract
                return (TComponent)CreateInstance(type.AssemblyQualifiedName, args);
            }

            throw new ComponentBrokerException(string.Format("Could not create the component of type '{0}'", type.AssemblyQualifiedName));
        }

        /// <summary>
        /// Find the assembly that the given type exists in 
        /// and create an instance of it.
        /// </summary>
        /// <param name="type">Fully qualified name of a type</param>
        /// <param name="args">Constructor parameters for the component to be initialized</param>
        /// <exception cref="ArgumentException">Key must not be null or empty.</exception>
        /// <exception cref="ComponentBrokerException">If the component could not be created.</exception>
        /// <returns></returns>
        private static object CreateInstance(string type, params object[] args)
        {
            if (string.IsNullOrEmpty(type)) throw new ArgumentException("Type must not be null or empty.", "type");

            // get the type
            var t = Type.GetType(type);

            //if the type is valid
            if (t != null)
            {
                if (args != null && args.Length > 0)
                {
                    return Activator.CreateInstance(t, args);
                }
                else
                {
                    return Activator.CreateInstance(t);
                }
            }
            
            throw new ComponentBrokerException(string.Format("Could not create the component of type '{0}'", type));
        }

        /// <summary>
        /// Retrieve the component registered to the given key.
        /// Cast it to the provided type.
        /// </summary>
        /// <typeparam name="T">Type of component</typeparam>
        /// <param name="key">Key the component is registered for</param>
        /// <exception cref="ArgumentException">Key must not be null or empty.</exception>
        /// <exception cref="ComponentBrokerException">No component is registered for the key.</exception>
        /// <returns>The registered component</returns>
        public static T RetrieveComponent<T>(string key)
        {
            return (T) RetrieveComponent(key);
        }

        /// <summary>
        /// Retrieve the component registered to the given key.
        /// Used when the type is not known.
        /// </summary>
        /// <param name="key">Key the component is registered for</param>
        /// <exception cref="ArgumentException">Key must not be null or empty.</exception>
        /// <exception cref="ComponentBrokerException">No component is registered for the key.</exception>
        /// <returns>The registered component</returns>
        public static object RetrieveComponent(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key must not be null or empty.", "key");

            if (HasComponent(key))
            {
                return Components[key];
            }

            throw new ComponentBrokerException(string.Format("No component is registered for the key '{0}'", key));
        }

        #endregion RETRIEVE COMPONENT

        #region UNREGISTER COMPONENTS

        /// <summary>
        /// Unregister a specific component that is registered for the given type.
        /// </summary>
        /// <typeparam name="TComponent">Type of component</typeparam>
        /// <exception cref="ArgumentException">Key must not be null or empty.</exception>
        public static void UnregisterComponent<TComponent>()
        {
            UnregisterComponent(typeof(TComponent).AssemblyQualifiedName);
        }

        /// <summary>
        /// Unregister a specific component that is registered for the given key.
        /// </summary>
        /// <param name="key">Key the component is registered for</param>
        /// <exception cref="ArgumentException">Key must not be null or empty.</exception>
        public static void UnregisterComponent(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key must not be null or empty.", "key");

            if (HasComponent(key))
            {
                Components.Remove(key);
            }
        }

        /// <summary>
        /// Unregister all components.
        /// </summary>
        [Obsolete("Use UnregisterAllComponents", true)]
        public static void UnregisterAll()
        {
            UnregisterAllComponents();
        }

        /// <summary>
        /// Unregister all components.
        /// </summary>
        public static void UnregisterAllComponents()
        {
            Components.Clear();
        }

        #endregion UNREGISTER COMPONENTS

        #endregion

        /// <summary>
        /// Unregister everything, reseting the component broker 
        /// back to its initial state.
        /// </summary>
        public static void Reset()
        {
            UnregisterAllFactories();
            UnregisterAllTypes();
            UnregisterAllComponents();
            InterfaceRegexMask = Standardinterfacemask;
        }
    }
}
