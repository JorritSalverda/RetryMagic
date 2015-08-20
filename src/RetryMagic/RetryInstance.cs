using System;

namespace RetryMagic
{
    /// <summary>
    /// Used when needing separate instances of Retry class with settings; otherwise use the static class and methods.
    /// </summary>    
    public class RetryInstance:IRetryInstance
    {
        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public RetrySettings Settings { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryInstance"/> class.
        /// </summary>
        /// <param name="settings">Object that contain settings.</param>
        public RetryInstance(RetrySettings settings)
        {
            UpdateSettings(settings);
        }

        /// <summary>
        /// Updates the settings.
        /// </summary>
        /// <param name="settings">Settings.</param>
        public void UpdateSettings(RetrySettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }                 

            Settings = settings;
        }

        /// <summary>
        /// Retry a function using the values provided in the constructor.
        /// </summary>
        /// <typeparam name="T">The generic type of the result.</typeparam>
        /// <param name="functionToTry">Any function with a return value; for a function with parameters use a lambda expression.</param>
        /// <returns>The result the called function will return.</returns>
        public T Function<T>(Func<T> functionToTry)
        {
            return Retry.Function(functionToTry, Settings);
        }

        /// <summary>
        /// Retry an action using the values provided in the constructor.
        /// </summary>
        /// <param name="actionToTry">Any action.</param>
        public void Action(Action actionToTry)
        {
            Retry.Action(actionToTry, Settings);
        }
    }
}