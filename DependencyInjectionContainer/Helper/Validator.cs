using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using DependencyInjectionContainer.Api;
using DependencyInjectionContainer.Api.Validator;

namespace DependencyInjectionContainer.Helper
{
    /// <summary>
    ///     Configuration validator
    /// </summary>
    public class Validator : IValidator
    {
        /// <summary>
        ///     Dependencies configuration
        /// </summary>
        private readonly IDependenciesConfiguration Configuration;

        /// <summary>
        ///     Nested types
        /// </summary>
        private readonly Stack<Type> NestedTypes;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// 
        /// <param name="configuration">Dependencies configuration</param>
        public Validator(IDependenciesConfiguration configuration)
        {
            Configuration = configuration;
            NestedTypes = new Stack<Type>();
        }

        /// <summary>
        ///     Check if configuration is valid
        /// </summary>
        /// 
        /// <returns>bool</returns>
        public bool IsValid()
        {
            return Configuration.Dependencies.Values.All(
                implementations => implementations.All(
                    implementation => CanBeCreatedCheck(implementation.ImplementationType)
                    )
                );
        }

        /// <summary>
        ///     Check that intance can be created
        /// </summary>
        /// 
        /// <param name="instanceType">Instance type</param>
        /// 
        /// <returns>bool</returns>
        private bool CanBeCreatedCheck(Type instanceType)
        {
            NestedTypes.Push(instanceType);
            
            // Get all constructors
            var constructors = instanceType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            foreach (var constructor in constructors)
            {
                // Get all parameters in current constructor
                var parameters = constructor.GetParameters();
                foreach (var parameter in parameters)
                {
                    Type parameterType;
                    
                    // Check that constructor contains inteface types
                    if (parameter.ParameterType.ContainsGenericParameters)
                    {
                        parameterType = parameter.ParameterType.GetInterfaces()[0];
                    }
                    // Check that constructor contains generic types
                    else if (parameter.ParameterType.GetInterfaces().Any(i => i.Name == "IEnumerable"))
                    {
                        parameterType = parameter.ParameterType.GetGenericArguments()[0];
                    }
                    // Other cases
                    else
                    {
                        parameterType = parameter.ParameterType;
                    }

                    // Check that necessary type exists in container
                    if (parameterType.IsInterface && IsExists(parameterType))
                    {
                        continue;
                    }

                    NestedTypes.Pop();

                    return false;
                }
            }

            NestedTypes.Pop();

            return true;
        }

        /// <summary>
        ///     Check that necessary type exists in container
        /// </summary>
        /// 
        /// <param name="type">Type</param>
        /// 
        /// <returns>bool</returns>
        private bool IsExists(Type type)
        {
            return Configuration.Dependencies.ContainsKey(type);
        }
    }
}
