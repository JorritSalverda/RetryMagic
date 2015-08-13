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
        /// Determines the maximum number of attempts to retry for.
        /// </summary>
        public static int DefaultMaximumNumberOfAttempts { get; set; }

        /// <summary>
        /// With each attempt the amount of slots doubles; this is the time per slot.
        /// </summary>
        public static int MillisecondsPerSlot { get; set; }

        /// <summary>
        /// Determines whether the exponential backoff truncates or continuous to expand.
        /// </summary>
        public static bool TruncateNumberOfSlots { get; set; }

        /// <summary>
        /// If <see cref="Truncate"/> is true this maximizes the number of slots.
        /// </summary>
        public static int MaximumNumberOfSlotsWhenTruncated { get; set; }

        /// <summary>
        /// Gets or sets the jitter percentage used for applying jitter to the retry interval.
        /// </summary>
        /// <value>The jitter percentage.</value>
        public static int JitterPercentage { get; set; }

        /// <summary>
        /// Initialize the DefaultMaximumNumberOfAttempts; it can be overriden by the consumer of this class at application startup.
        /// </summary>
        static Retry()
        {
            DefaultMaximumNumberOfAttempts = 8;
            MillisecondsPerSlot = 32;
            TruncateNumberOfSlots = true;
            MaximumNumberOfSlotsWhenTruncated = 16;
            JitterPercentage = 25;
        }

        /// <summary>
        /// Retry a function with default settings.
        /// </summary>
        /// <typeparam name="T">The generic type of the result.</typeparam>
        /// <param name="functionToTry">Any function with a return value; for a function with parameters use a lambda expression.</param>
        /// <returns>The result the called function will return.</returns>
        public static T Function<T>(Func<T> functionToTry)
        {
            return Function(functionToTry, DefaultMaximumNumberOfAttempts, MillisecondsPerSlot, TruncateNumberOfSlots, MaximumNumberOfSlotsWhenTruncated, JitterPercentage);
        }

        /// <summary>
        /// Retry a function with passed in settings.
        /// </summary>
        /// <typeparam name="T">The generic type of the result.</typeparam>
        /// <param name="functionToTry">Any function with a return value; for a function with parameters use a lambda expression.</param>
        /// <param name="maximumNumberOfAttempts">Maximum number of attempts.</param>
        /// <param name="millisecondsPerSlot">Milliseconds per slot.</param>
        /// <param name="truncate">If set to <c>true</c> truncate the number of slots.</param>
        /// <param name="maximumNumberOfSlotsWhenTruncated">Maximum number of slots when <see cref="truncate"/> is set to <c>true</c>.</param>
        /// <param name="jitterPercentage">Jitter percentage.</param>
        /// <returns>The result the called function will return.</returns>
        public static T Function<T>(Func<T> functionToTry, int maximumNumberOfAttempts, int millisecondsPerSlot, bool truncateNumberOfSlots, int maximumNumberOfSlotsWhenTruncated, int jitterPercentage)
        {
            ValidateParameters (maximumNumberOfAttempts, millisecondsPerSlot, truncateNumberOfSlots, maximumNumberOfSlotsWhenTruncated, jitterPercentage);

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
                    Thread.Sleep(GetIntervalInMilliseconds(numberOfAttempts, millisecondsPerSlot, truncateNumberOfSlots, maximumNumberOfSlotsWhenTruncated, jitterPercentage));
                }

                numberOfAttempts++;
            }

            var exceptionMessage = string.Format("Trying function {0} failed for {1} attempts.", functionToTry, maximumNumberOfAttempts);
            throw new AggregateException(exceptionMessage, innerExceptions);
        }

        /// <summary>
        /// Retry an action with default settings.
        /// </summary>
        /// <param name="actionToTry">Any action.</param>
        public static void Action(Action actionToTry)
        {
            Action(actionToTry, DefaultMaximumNumberOfAttempts, MillisecondsPerSlot, TruncateNumberOfSlots, MaximumNumberOfSlotsWhenTruncated, JitterPercentage);
        }

        /// <summary>
        /// Retry an action with passed in settings.
        /// </summary>
        /// <param name="actionToTry">Any action.</param>
        /// <param name="maximumNumberOfAttempts">Maximum number of attempts.</param>
        /// <param name="millisecondsPerSlot">Milliseconds per slot.</param>
        /// <param name="truncate">If set to <c>true</c> truncate the number of slots.</param>
        /// <param name="maximumNumberOfSlotsWhenTruncated">Maximum number of slots when <see cref="truncate"/> is set to <c>true</c>.</param>
        /// <param name="jitterPercentage">Jitter percentage.</param>
        public static void Action(Action actionToTry, int maximumNumberOfAttempts, int millisecondsPerSlot, bool truncateNumberOfSlots, int maximumNumberOfSlotsWhenTruncated, int jitterPercentage)
        {
            ValidateParameters (maximumNumberOfAttempts, millisecondsPerSlot, truncateNumberOfSlots, maximumNumberOfSlotsWhenTruncated, jitterPercentage);

            var innerExceptions = new List<Exception>();
            var numberOfAttempts = 0;

            while (numberOfAttempts < maximumNumberOfAttempts)
            {
                try
                {
                    actionToTry.Invoke();

                    return;
                }
                catch (Exception exception)
                {
                    innerExceptions.Add(exception);
                    Thread.Sleep(GetIntervalInMilliseconds(numberOfAttempts, millisecondsPerSlot, truncateNumberOfSlots, maximumNumberOfSlotsWhenTruncated, jitterPercentage));
                }

                numberOfAttempts++;
            }

            var exceptionMessage = string.Format("Trying action {0} failed for {1} attempts.", actionToTry, maximumNumberOfAttempts);
            throw new AggregateException(exceptionMessage, innerExceptions);
        }

        /// <summary>
        /// Per attempt the number of slots doubles; the number of slots either continuous to rise or is truncate if <see cref="Truncate"/> is true and the slots grow passed MaximumNumberOfSlotsWhenTruncated.
        /// </summary>
        /// <returns>The interval in milliseconds.</returns>
        /// <param name="numberOfAttempts">The xth attempt.</param>
        /// <param name="millisecondsPerSlot">Milli seconds per slot.</param>
        /// <param name="truncate">If set to <c>true</c> truncate the number of slots.</param>
        /// <param name="maximumNumberOfSlotsWhenTruncated">Maximum number of slots when <see cref="truncate"/> is set to <c>true</c>.</param>
        /// <param name="jitterPercentage">Jitter percentage.</param>
        private static int GetIntervalInMilliseconds(int numberOfAttempts, int millisecondsPerSlot, bool truncate, int maximumNumberOfSlotsWhenTruncated, int jitterPercentage)
        {
            // binary exponential backoff
            var numberOfSlots = Math.Pow(2, numberOfAttempts) - 1;

            // truncate if Truncate is true, otherwise cap at int.MaxValue
            var maximumNumberOfSlots = truncate ? maximumNumberOfSlotsWhenTruncated : int.MaxValue;
            var numberOfSlotsAsInteger = numberOfSlots > maximumNumberOfSlots ? maximumNumberOfSlots : (int)numberOfSlots;

            // multiply slots times MilliSecondsPerSlot; apply jitter to the resulting time in order to increase entropy in your system
            return Jitter.Apply(numberOfSlotsAsInteger * millisecondsPerSlot, jitterPercentage);
        }

        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="maximumNumberOfAttempts">Maximum number of attempts.</param>
        /// <param name="millisecondsPerSlot">Milliseconds per slot.</param>
        /// <param name="truncate">If set to <c>true</c> truncate the number of slots.</param>
        /// <param name="maximumNumberOfSlotsWhenTruncated">Maximum number of slots when <see cref="truncate"/> is set to <c>true</c>.</param>
        /// <param name="jitterPercentage">Jitter percentage.</param>
        internal static void ValidateParameters(int maximumNumberOfAttempts, int millisecondsPerSlot, bool truncateNumberOfSlots, int maximumNumberOfSlotsWhenTruncated, int jitterPercentage)
        {
            if (maximumNumberOfAttempts < 1)
            {
                throw new ArgumentOutOfRangeException("maximumNumberOfAttempts", "Maximum number of attempts needs to be 1 or larger");
            }
            if (millisecondsPerSlot < 1)
            {
                throw new ArgumentOutOfRangeException("milliSecondsPerSlot", "Milliseconds per slot needs to be 1 or larger");
            }
            if (maximumNumberOfSlotsWhenTruncated < 1)
            {
                throw new ArgumentOutOfRangeException("maximumNumberOfSlotsWhenTruncated", "Maximum number of slots when truncated needs to be 1 or larger");
            }
            if (jitterPercentage < 0)
            {
                throw new ArgumentOutOfRangeException("jitterPercentage", "Jitter percentage needs to be 0 or larger");
            }
        }
    }
}