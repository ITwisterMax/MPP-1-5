using System;
using DependencyInjectionContainer.Api.Parameters;

namespace DependencyInjectionContainer.Model
{
    /// <summary>
    ///     Implementation container
    /// </summary>
    public class Implementation
    {
        /// <summary>
        ///     Implementation type
        /// </summary>
        public Type ImplementationType { get; }

        /// <summary>
        ///     Time to live
        /// </summary>
        public TTL TimeToLive { get; }

        /// <summary>
        ///     Implementation number
        /// </summary>
        public ServiceImplementations Number { get; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// 
        /// <param name="implementationType">Implementation type</param>
        /// <param name="ttl">Time to live</param>
        /// <param name="number">Implementation number</param>
        public Implementation(Type implementationType, TTL ttl, ServiceImplementations number)
        {
            ImplementationType = implementationType;
            TimeToLive = ttl;
            Number = number;
        }
    }
}
