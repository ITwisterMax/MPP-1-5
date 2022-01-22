using System;
using DependencyInjectionContainer.Api.Parameters;

namespace DependencyInjectionContainer.Api.Attributes
{
    /// <summary>
    ///     Custom attribute for dependency provider
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class DependencyKeyAttribute : Attribute
    {
        /// <summary>
        ///     Implementation number
        /// </summary>
        public ServiceImplementations Number { get; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// 
        /// <param name="number">Implementation number</param>
        public DependencyKeyAttribute(ServiceImplementations number)
        {
            Number = number;
        }
    }
}
