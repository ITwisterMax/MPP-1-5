using System;

namespace DependencyInjectionContainer.Api.Parameters
{
    /// <summary>
    ///     Implementation number
    /// </summary>
    [Flags]
    public enum ServiceImplementations
    {
        None = 1,
        First = 2,
        Second = 4,
        Any = None | First | Second
    }
}
