using System;
using System.Collections.Generic;
using DependencyInjectionContainer.Api.Parameters;
using DependencyInjectionContainer.Model;

namespace DependencyInjectionContainer.Api
{
    /// <summary>
    ///     Dependencies configuration interface
    /// </summary>
    public interface IDependenciesConfiguration
    {
        /// <summary>
        ///     Dependencies dictionary
        /// </summary>
        public Dictionary<Type, List<Implementation>> Dependencies { get; }

        /// <summary>
        ///     Register a new dependency
        /// </summary>
        /// 
        /// <typeparam name="TDependency">Dependency class</typeparam>
        /// <typeparam name="TImplementation">Implementation class</typeparam>
        /// 
        /// <param name="ttl">Time to live</param>
        /// <param name="number">Implementation number</param>
        void Register<TDependency, TImplementation>(TTL ttl, ServiceImplementations number = ServiceImplementations.None)
            where TDependency : class
            where TImplementation : TDependency;

        /// <summary>
        ///     Register a new dependency
        /// </summary>
        /// 
        /// <param name="dependencyType">Dependency class</param>
        /// <param name="implementationType">Implementation class</param>
        /// <param name="ttl">Time to live</param>
        /// <param name="number">Implementation number</param>
        void Register(Type dependencyType, Type implementationType, TTL ttl, ServiceImplementations number = ServiceImplementations.None);
    }
}
