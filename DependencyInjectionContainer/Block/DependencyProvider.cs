using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DependencyInjectionContainer.Api;
using DependencyInjectionContainer.Api.Attributes;
using DependencyInjectionContainer.Api.Parameters;
using DependencyInjectionContainer.Api.Validator;
using DependencyInjectionContainer.Helper;
using DependencyInjectionContainer.Model;

namespace DependencyInjectionContainer.Block
{
    /// <summary>
    ///     Dependency provider
    /// </summary>
    public class DependencyProvider : IDependencyProvider
    {
        /// <summary>
        ///     Dependencies configuration
        /// </summary>
        private readonly IDependenciesConfiguration Configuration;

        /// <summary>
        ///     Singleton objects
        /// </summary>
        private readonly Dictionary<Type, List<Singleton>> Singletons;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// 
        /// <param name="configuration">Dependencies configuration</param>
        public DependencyProvider(IDependenciesConfiguration configuration)
        {
            // Check if configuration is valid
            IValidator configValidator = new Validator(configuration);
            if (!configValidator.IsValid())
            {
                throw new ArgumentException("Invalid configuration!");
            }

            Configuration = configuration;
            Singletons = new Dictionary<Type, List<Singleton>>();
        }

        /// <summary>
        ///     Resolve a dependency
        /// </summary>
        /// 
        /// <typeparam name="TDependency">Dependency class</typeparam>
        /// <param name="number">Implementation number</param>
        /// 
        /// <returns>TDependency</returns>
        public TDependency Resolve<TDependency>(ServiceImplementations number = ServiceImplementations.Any)
            where TDependency : class
        {
            return (TDependency)Resolve(typeof(TDependency), number);
        }

        /// <summary>
        ///     Resolve a dependency
        /// </summary>
        /// 
        /// <param name="dependencyType">Dependency class</param>
        /// <param name="number">Implementation number</param>
        /// 
        /// <returns>object</returns>
        public object Resolve(Type dependencyType, ServiceImplementations number = ServiceImplementations.Any)
        {
            object result;

            // Case if dependency type is enumerable
            if (IsEnumerable(dependencyType))
            {
                result = CreateEnumerable(dependencyType.GetGenericArguments()[0]);
            }
            // Other cases
            else
            {
                Implementation container = GetContainerByDependencyType(dependencyType, number);
                Type type = GetGeneratedType(dependencyType, container.ImplementationType);

                result = ResolveNonEnumerable(type, container.TimeToLive, dependencyType, container.Number);
            }

            return result;
        }

        /// <summary>
        ///     Check that dependency type is enumerable
        /// </summary>
        /// 
        /// <param name="dependencyType">Dependency type</param>
        /// 
        /// <returns>bool</returns>
        private bool IsEnumerable(Type dependencyType)
        {
            return dependencyType.GetInterfaces().Any(i => i.Name == "IEnumerable");
        }

        /// <summary>
        ///     Create dependency for enumerable object
        /// </summary>
        /// <param name="dependencyType">Dependency type</param>
        /// 
        /// <returns>IList</returns>
        private IList CreateEnumerable(Type dependencyType)
        {
            var result = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(dependencyType));
            var containers = Configuration.Dependencies[dependencyType];

            // Resolve all base types and add them to list
            foreach (var container in containers)
            {
                var instance = ResolveNonEnumerable(container.ImplementationType, container.TimeToLive, dependencyType, container.Number);
                result.Add(instance);
            }

            return result;
        }

        /// <summary>
        ///     Get container by dependency type
        /// </summary>
        /// 
        /// <param name="dependencyType">Dependency type</param>
        /// <param name="number">Implementation number</param>
        /// 
        /// <returns>Implementation</returns>
        private Implementation GetContainerByDependencyType(Type dependencyType, ServiceImplementations number)
        {
            Implementation container;

            // Case if dependency type is generics
            if (dependencyType.IsGenericType)
            {
                container = GetLastContainer(dependencyType, number);
                container ??= GetLastContainer(dependencyType.GetGenericTypeDefinition(), number);
            }
            // Other cases
            else
            {
                container = GetLastContainer(dependencyType, number);
            }

            return container;
        }

        /// <summary>
        ///     Get generated class
        /// </summary>
        /// 
        /// <param name="dependencyType">Dependency type</param>
        /// <param name="implementationType">Implementation type</param>
        /// 
        /// <returns>Type</returns>
        private Type GetGeneratedType(Type dependencyType, Type implementationType)
        {
            // Case if dependency type is generics
            if (dependencyType.IsGenericType && implementationType.IsGenericTypeDefinition)
            {
                return implementationType.MakeGenericType(dependencyType.GetGenericArguments());
            }

            return implementationType;
        }

        /// <summary>
        ///     Resolve non enumerable class
        /// </summary>
        /// 
        /// <param name="implementationType">Implementation type</param>
        /// <param name="ttl">Time to live</param>
        /// <param name="dependencyType">Dependency type</param>
        /// <param name="number">Implementation number</param>
        /// 
        /// <returns>object</returns>
        private object ResolveNonEnumerable(Type implementationType, TTL ttl, Type dependencyType, ServiceImplementations number)
        {
            // Case if it isn't singleton
            if (ttl != TTL.Singleton)
            {
                return CreateInstance(implementationType);
            }

            // Other case (lock for multi thread)
            lock (Configuration)
            {
                // Case if singleton exists
                if (IsInSingletons(dependencyType, implementationType, number))
                {
                    return Singletons[dependencyType].Find(singletonContainer => number.HasFlag(singletonContainer.Number)).Instance;
                }

                // Create a new singleton object
                var result = CreateInstance(implementationType);
                AddToSingletons(dependencyType, result, number);

                return result;
            }
        }

        /// <summary>
        ///     Get last container
        /// </summary>
        /// 
        /// <param name="dependencyType">Dependency type</param>
        /// <param name="number">Implementation number</param>
        /// 
        /// <returns>Implementation</returns>
        private Implementation GetLastContainer(Type dependencyType, ServiceImplementations number)
        {
            if (Configuration.Dependencies.ContainsKey(dependencyType))
            {
                return Configuration.Dependencies[dependencyType].FindLast(container => number.HasFlag(container.Number));
            }

            return null;
        }

        /// <summary>
        ///     Check that singleton exists
        /// </summary>
        /// 
        /// <param name="dependencyType">Dependency type</param>
        /// <param name="implementationType">Implementation type</param>
        /// <param name="number">Implementation number</param>
        /// 
        /// <returns>bool</returns>
        private bool IsInSingletons(Type dependencyType, Type implementationType, ServiceImplementations number)
        {
            var list = Singletons.ContainsKey(dependencyType) ? Singletons[dependencyType] : null;

            return list?.Find(container => number.HasFlag(container.Number) && container.Instance.GetType() == implementationType) != null;
        }

        /// <summary>
        ///     Add a new sigleton object
        /// </summary>
        /// 
        /// <param name="dependencyType">Dependency type</param>
        /// <param name="implementation">Implementation type</param>
        /// <param name="number">Implementation number</param>
        private void AddToSingletons(Type dependencyType, object implementation, ServiceImplementations number)
        {
            if (Singletons.ContainsKey(dependencyType))
            {
                Singletons[dependencyType].Add(new Singleton(implementation, number));
            }
            else
            {
                Singletons.Add(dependencyType, new List<Singleton>(){ new Singleton(implementation, number) });
            }
        }

        /// <summary>
        ///     Create a new dependency instance
        /// </summary>
        /// 
        /// <param name="implementationType">Implementation type</param>
        /// 
        /// <returns>object</returns>
        private object CreateInstance(Type implementationType)
        {
            var constructors = implementationType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            foreach (var constructor in constructors)
            {
                var generatedParams = new List<dynamic>();

                // Get all parameters and proccess nested types
                var constructorParams = constructor.GetParameters();
                foreach (var parameterInfo in constructorParams)
                {
                    dynamic parameter;
                    if (parameterInfo.ParameterType.IsInterface)
                    {
                        var number = parameterInfo.GetCustomAttribute<DependencyKeyAttribute>()?.Number ?? ServiceImplementations.Any;
                        parameter = Resolve(parameterInfo.ParameterType, number);
                    }
                    else
                    {
                        break;
                    }

                    generatedParams.Add(parameter);
                }

                return constructor.Invoke(generatedParams.ToArray());
            }

            throw new ArgumentException("Can't create class instance!");
        }        
    }
}
