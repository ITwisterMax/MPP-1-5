using System;
using System.Collections.Generic;
using System.Linq;
using DependencyInjectionContainer.Api;
using DependencyInjectionContainer.Api.Parameters;
using DependencyInjectionContainer.Model;

namespace DependencyInjectionContainer.Block
{
    /// <summary>
    ///     Dependencies configuration interface
    /// </summary>
    public class DependenciesConfiguration : IDependenciesConfiguration
    {
        /// <summary>
        ///     Dependencies dictionary
        /// </summary>
        public Dictionary<Type, List<Implementation>> Dependencies { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public DependenciesConfiguration()
        {
            Dependencies = new Dictionary<Type, List<Implementation>>();
        }

        /// <summary>
        ///     Register a new dependency
        /// </summary>
        /// 
        /// <typeparam name="TDependency">Dependency class</typeparam>
        /// <typeparam name="TImplementation">Implementation class</typeparam>
        /// 
        /// <param name="ttl">Time to live</param>
        /// <param name="number">Implementation number</param>
        public void Register<TDependency, TImplementation>(TTL ttl, ServiceImplementations number = ServiceImplementations.None) 
            where TDependency : class 
            where TImplementation : TDependency
        {
            Register(typeof(TDependency), typeof(TImplementation), ttl, number);
        }

        /// <summary>
        ///     Register a new dependency
        /// </summary>
        /// 
        /// <param name="dependencyType">Dependency class</param>
        /// <param name="implementationType">Implementation class</param>
        /// <param name="ttl">Time to live</param>
        /// <param name="number">Implementation number</param>
        public void Register(Type dependencyType, Type implementType, TTL ttl, ServiceImplementations number = ServiceImplementations.None)
        {
            // Check that implementation and dependency are compatible
            if (!IsCompatible(dependencyType, implementType))
            {
                throw new ArgumentException("Incompatible types!");
            }

            var container = new Implementation(implementType, ttl, number);

            // Check that dependency has already exist
            if (Dependencies.ContainsKey(dependencyType))
            {
                var index = Dependencies[dependencyType].FindIndex(elem => elem.ImplementationType == container.ImplementationType);
                
                // Remove a duplicate dependency
                if (index != -1)
                {
                    Dependencies[dependencyType].RemoveAt(index);
                }

                // Add a new dependency
                Dependencies[dependencyType].Add(container);

            }
            // Add a new dependency
            else
            {
                Dependencies.Add(dependencyType, new List<Implementation>() { container });
            }
        }

        /// <summary>
        ///     Check that implementation and dependency are compatible
        /// </summary>
        /// 
        /// <param name="implementationType">Implementation class</param>
        /// <param name="dependencyType">Dependency class</param>
        /// 
        /// <returns>bool</returns>
        private bool IsCompatible(Type dependencyType, Type implementationType)
        {
            return implementationType.IsAssignableFrom(dependencyType) ||
                implementationType.GetInterfaces().Any(i => i.ToString() == dependencyType.ToString());
        }
    }
}
