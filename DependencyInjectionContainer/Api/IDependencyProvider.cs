using System;
using DependencyInjectionContainer.Api.Parameters;

namespace DependencyInjectionContainer.Api
{
    /// <summary>
    ///     Dependency provider interface
    /// </summary>
    public interface IDependencyProvider
    {
        /// <summary>
        ///     Resolve a dependency
        /// </summary>
        /// 
        /// <typeparam name="TDependency">Dependency class</typeparam>
        /// <param name="number">Implementation number</param>
        /// 
        /// <returns>TDependency</returns>
        TDependency Resolve<TDependency>(ServiceImplementations number = ServiceImplementations.Any)
            where TDependency : class;

        /// <summary>
        ///     Resolve a dependency
        /// </summary>
        /// 
        /// <param name="dependencyType">Dependency class</param>
        /// <param name="number">Implementation number</param>
        /// 
        /// <returns>object</returns>
        object Resolve(Type dependencyType, ServiceImplementations number = ServiceImplementations.Any);
    }
}
