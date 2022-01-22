using DependencyInjectionContainer.Api.Parameters;

namespace DependencyInjectionContainer.Model
{
    /// <summary>
    ///     Singleton container
    /// </summary>
    public class Singleton
    {
        /// <summary>
        ///     Object instance
        /// </summary>
        public readonly object Instance;

        /// <summary>
        ///     Implementation number
        /// </summary>
        public readonly ServiceImplementations Number;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// 
        /// <param name="instance">Object instance</param>
        /// <param name="number">Implementation number</param>
        public Singleton(object instance, ServiceImplementations number)
        {
            Instance = instance;
            Number = number;
        }
    }
}
