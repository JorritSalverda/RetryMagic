using System;

namespace RetryMagic
{
    /// <summary>
    /// Used when needing separate instances of Retry class with settings; otherwise use the static class and methods.
    /// </summary>    
    public interface IRetryInstance
    {
        /// <summary>
        /// Settings for this instance.
        /// </summary>
        /// <value>The settings.</value>
        RetrySettings Settings { get; }

        /// <summary>
        /// Updates the settings.
        /// </summary>
        /// <param name="settings">Settings.</param>
        void UpdateSettings(RetrySettings settings);

        /// <summary>
        /// Retry a function using the values provided in the constructor.
        /// </summary>
        /// <typeparam name="T">The generic type of the result</typeparam>
        /// <param name="functionToTry">Any function with a return value; for a function with parameters use a lambda expression.</param>
        /// <returns>The result the called function will return.</returns>
        T Function<T>(Func<T> functionToTry);

        /// <summary>
        /// Retry an action using the values provided in the constructor.
        /// </summary>
        /// <param name="actionToTry">Any action.</param>
        void Action(Action actionToTry);
    }
}