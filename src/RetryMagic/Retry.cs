using System;
using System.Collections.Generic;
using System.Threading;
using JitterMagic;

namespace RetryMagic
{
    /// <summary>
    /// Class to retry Actions or Functions with (truncated) binary exponential backoff
    /// </summary>
    public static class Retry
    {
        /// <summary>
        /// Determines the maximum number of attempts to retry for
        /// </summary>
        public static int DefaultMaximumNumberOfAttempts { get; set; }

        /// <summary>
        /// With each attempt the amount of slots doubles; this is the time per slot
        /// </summary>
        public static int MilliSecondsPerSlot { get; set; }

        /// <summary>
        /// Determines whether the exponential backoff truncates or continuous to expand
        /// </summary>
        public static bool Truncate { get; set; }

        /// <summary>
        /// If <see cref="Truncate"/> is true this maximizes the number of slots
        /// </summary>
        public static int MaximumNumberOfSlotsWhenTruncated { get; set; }

        /// <summary>
        /// Initialize the DefaultMaximumNumberOfAttempts; it can be overriden by the consumer of this class at application startup
        /// </summary>
        static Retry()
        {
            DefaultMaximumNumberOfAttempts = 8;
            MilliSecondsPerSlot = 32;
            Truncate = true;
            MaximumNumberOfSlotsWhenTruncated = 16;
        }

        /// <summary>
        /// Retry a function for a maximum of DefaultMaximumNumberOfAttempts attempts in case of failure
        /// </summary>
        /// <typeparam name="T">The generic type of the result</typeparam>
        /// <param name="functionToTry">Any function with a return value; for a function with parameters use a lambda expression</param>
        /// <returns>The result the called function will return</returns>
        public static T Function<T>(Func<T> functionToTry)
        {
            return Function(functionToTry, DefaultMaximumNumberOfAttempts);
        }

        /// <summary>
        /// Retry a function for a maximum of <see cref="maximumNumberOfAttempts"/> attempts in case of failure
        /// </summary>
        /// <typeparam name="T">The generic type of the result</typeparam>
        /// <param name="functionToTry">Any function with a return value; for a function with parameters use a lambda expression</param>
        /// <param name="maximumNumberOfAttempts">An integer larger than 1</param>
        /// <returns>The result the called function will return</returns>
        public static T Function<T>(Func<T> functionToTry, int maximumNumberOfAttempts)
        {
            var innerExceptions = new List<Exception>();
            var numberOfAttempts = 0;

            while (numberOfAttempts < maximumNumberOfAttempts)
            {
                try
                {
                    var result = functionToTry.Invoke();

                    return result;
                }
                catch (Exception exception)
                {
                    innerExceptions.Add(exception);
                    Thread.Sleep(GetIntervalInMilliseconds(numberOfAttempts));
                }

                numberOfAttempts++;
            }

            var exceptionMessage = string.Format("Trying function {0} failed for {1} attempts.", functionToTry, maximumNumberOfAttempts);
            throw new AggregateException(exceptionMessage, innerExceptions);
        }

        /// <summary>
        /// Retry an action for a maximum of DefaultMaximumNumberOfAttempts attempts in case of failure
        /// </summary>
        /// <param name="actionToTry">Any action</param>
        public static void Action(Action actionToTry)
        {
            Action(actionToTry, DefaultMaximumNumberOfAttempts);
        }

        /// <summary>
        /// Retry an action for a maximum of DefaultMaximumNumberOfAttempts attempts in case of failure
        /// </summary>
        /// <param name="actionToTry">Any action</param>
        /// <param name="maximumNumberOfAttempts">An integer larger than 1</param>
        public static void Action(Action actionToTry, int maximumNumberOfAttempts)
        {
            var innerExceptions = new List<Exception>();
            var numberOfAttempts = 0;

            while (numberOfAttempts < DefaultMaximumNumberOfAttempts)
            {
                try
                {
                    actionToTry.Invoke();

                    return;
                }
                catch (Exception exception)
                {
                    innerExceptions.Add(exception);
                    Thread.Sleep(GetIntervalInMilliseconds(numberOfAttempts));
                }

                numberOfAttempts++;
            }

            var exceptionMessage = string.Format("Trying action {0} failed for {1} attempts.", actionToTry, DefaultMaximumNumberOfAttempts);
            throw new AggregateException(exceptionMessage, innerExceptions);
        }

        /// <summary>
        /// Per attempt the number of slots doubles; the number of slots either continuous to rise or is truncate if <see cref="Truncate"/> is true and the slots grow passed MaximumNumberOfSlotsWhenTruncated
        /// </summary>
        /// <param name="numberOfAttempts">The xth attempt</param>
        /// <returns></returns>
        private static int GetIntervalInMilliseconds(int numberOfAttempts)
        {
            // binary exponential backoff
            var numberOfSlots = Math.Pow(2, numberOfAttempts) - 1;

            // truncate if Truncate is true, otherwise cap at int.MaxValue
            var maximumNumberOfSlots = Truncate ? MaximumNumberOfSlotsWhenTruncated : int.MaxValue;
            var numberOfSlotsAsInteger = numberOfSlots > maximumNumberOfSlots ? maximumNumberOfSlots : (int)numberOfSlots;

            // multiply slots times MilliSecondsPerSlot; apply jitter to the resulting time in order to increase entropy in your system
            return Jitter.Apply(numberOfSlotsAsInteger * MilliSecondsPerSlot);
        }
    }
}