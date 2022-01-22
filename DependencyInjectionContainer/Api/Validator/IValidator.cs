namespace DependencyInjectionContainer.Api.Validator
{
    /// <summary>
    ///     Configuration validator interface
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        ///     Check if configuration is valid
        /// </summary>
        /// 
        /// <returns>bool</returns>
        bool IsValid();
    }
}
