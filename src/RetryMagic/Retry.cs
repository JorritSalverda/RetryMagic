using System;
using System.Collections.Generic;
using System.Threading;
using JitterMagic;

namespace RetryMagic
{
    /// <summary>
    /// Class to retry Actions or Functions with (truncated) binary exponential backoff.
    /// </summary>
    public static class Retry
    {
        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public static RetrySettings Settings { get; private set; }

        /// <summary>
        /// Initialize the default settings; it can be overriden by the consumer of this class at application startup.
        /// </summary>
        static Retry()
        {
            Settings = new RetrySettings();
        }

        /// <summary>
        /// Updates the settings.
        /// </summary>
        /// <param name="settings">Settings.</param>
        public static void UpdateSettings(RetrySettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            Settings = settings;
        }

        /// <summary>
        /// Retry a function with default settings.
        /// </summary>
        /// <typeparam name="T">The generic type of the result.</typeparam>
        /// <param name="functionToTry">Any function with a return value; for a function with parameters use a lambda expression.</param>
        /// <returns>The result the called function will return.</returns>
        public static T Function<T>(Func<T> functionToTry)
        {
            return Function(functionToTry, Settings);
        }

        /// <summary>
        /// Retry a function with passed in settings.
        /// </summary>
        /// <typeparam name="T">The generic type of the result.</typeparam>
        /// <param name="functionToTry">Any function with a return value; for a function with parameters use a lambda expression.</param>
        /// <param name="settings">Object that contain settings.</param>
        /// <returns>The result the called function will return.</returns>
        public static T Function<T>(Func<T> functionToTry, RetrySettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            var innerExceptions = new List<Exception>();
            var numberOfAttempts = 0;

            while (numberOfAttempts < settings.MaximumNumberOfAttempts)
            {
                try
                {
                    var result = functionToTry.Invoke();

                    return result;
                }
                catch (Exception exception)
                {
                    innerExceptions.Add(exception);
                    Thread.Sleep(GetIntervalInMilliseconds(numberOfAttempts, settings));
                }

                numberOfAttempts++;
            }

            var exceptionMessage = string.Format("Trying function {0} failed for {1} attempts.", functionToTry, settings.MaximumNumberOfAttempts);
            throw new RetryException(exceptionMessage, innerExceptions);
        }

        /// <summary>
        /// Retry an action with default settings.
        /// </summary>
        /// <param name="actionToTry">Any action.</param>
        public static void Action(Action actionToTry)
        {
            Action(actionToTry, Settings);
        }

        /// <summary>
        /// Retry an action with passed in settings.
        /// </summary>
        /// <param name="actionToTry">Any action.</param>
        /// <param name="settings">Object that contain settings.</param>
        public static void Action(Action actionToTry, RetrySettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            var innerExceptions = new List<Exception>();
            var numberOfAttempts = 0;

            while (numberOfAttempts < settings.MaximumNumberOfAttempts)
            {
                try
                {
                    actionToTry.Invoke();

                    return;
                }
                catch (Exception exception)
                {
                    innerExceptions.Add(exception);
                    Thread.Sleep(GetIntervalInMilliseconds(numberOfAttempts, settings));
                }

                numberOfAttempts++;
            }

            var exceptionMessage = string.Format("Trying action {0} failed for {1} attempts.", actionToTry, settings.MaximumNumberOfAttempts);
            throw new RetryException(exceptionMessage, innerExceptions);
        }

        /// <summary>
        /// Per attempt the number of slots doubles; the number of slots either continuous to rise or is truncate if <see cref="Truncate"/> is true and the slots grow passed MaximumNumberOfSlotsWhenTruncated.
        /// </summary>
        /// <returns>The interval in milliseconds.</returns>
        /// <param name="numberOfAttempt">The xth attempt.</param>
        /// <param name="settings">Object that contain settings.</param>
        private static int GetIntervalInMilliseconds(int numberOfAttempt, RetrySettings settings)
        {
            // binary exponential backoff
            var numberOfSlots = Math.Pow(2, numberOfAttempt) - 1;

            // truncate if Truncate is true, otherwise cap at int.MaxValue
            var maximumNumberOfSlots = settings.TruncateNumberOfSlots ? settings.MaximumNumberOfSlotsWhenTruncated : int.MaxValue;
            var numberOfSlotsAsInteger = numberOfSlots > maximumNumberOfSlots ? maximumNumberOfSlots : (int)numberOfSlots;

            // multiply slots times MilliSecondsPerSlot; apply jitter to the resulting time in order to increase entropy in your system
            return Jitter.Apply(numberOfSlotsAsInteger * settings.MillisecondsPerSlot, settings.JitterSettings);
        }
    }
}