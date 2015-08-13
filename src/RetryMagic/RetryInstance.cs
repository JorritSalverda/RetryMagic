using System;

namespace RetryMagic
{
    /// <summary>
    /// Used when needing separate instances of Retry class with settings; otherwise use the static class and methods.
    /// </summary>    
    public class RetryInstance:IRetryInstance
    {
        /// <summary>
        /// Determines the maximum number of attempts to retry for.
        /// </summary>
        private int maximumNumberOfAttempts;

        /// <summary>
        /// With each attempt the amount of slots doubles; this is the time per slot.
        /// </summary>
        private int millisecondsPerSlot;

        /// <summary>
        /// Determines whether the exponential backoff truncates or continuous to expand.
        /// </summary>
        private bool truncateNumberOfSlots;

        /// <summary>
        /// If <see cref="Truncate"/> is true this maximizes the number of slots.
        /// </summary>
        private int maximumNumberOfSlotsWhenTruncated;

        /// <summary>
        /// Gets or sets the jitter percentage used for applying jitter to the retry interval.
        /// </summary>
        /// <value>The jitter percentage.</value>
        private int jitterPercentage;

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryInstance"/> class.
        /// </summary>
        /// <param name="maximumNumberOfAttempts">Maximum number of attempts.</param>
        /// <param name="milliSecondsPerSlot">Milliseconds per slot.</param>
        /// <param name="truncate">If set to <c>true</c> truncate the number of slots.</param>
        /// <param name="maximumNumberOfSlotsWhenTruncated">Maximum number of slots when <see cref="truncate"/> is set to <c>true</c>.</param>
        /// <param name="jitterPercentage">Jitter percentage.</param>
        public RetryInstance(int maximumNumberOfAttempts, int milliSecondsPerSlot, bool truncate, int maximumNumberOfSlotsWhenTruncated, int jitterPercentage)
        {
            Retry.ValidateParameters(maximumNumberOfAttempts, milliSecondsPerSlot, truncate, maximumNumberOfSlotsWhenTruncated, jitterPercentage);

            this.maximumNumberOfAttempts = maximumNumberOfAttempts;
            this.millisecondsPerSlot = milliSecondsPerSlot;
            this.truncateNumberOfSlots = truncate;
            this.maximumNumberOfSlotsWhenTruncated = maximumNumberOfSlotsWhenTruncated;
            this.jitterPercentage = jitterPercentage;
        }

        /// <summary>
        /// Retry a function using the values provided in the constructor.
        /// </summary>
        /// <typeparam name="T">The generic type of the result.</typeparam>
        /// <param name="functionToTry">Any function with a return value; for a function with parameters use a lambda expression.</param>
        /// <returns>The result the called function will return.</returns>
        public T Function<T>(Func<T> functionToTry)
        {
            return Retry.Function(functionToTry, maximumNumberOfAttempts, millisecondsPerSlot, truncateNumberOfSlots, maximumNumberOfSlotsWhenTruncated, jitterPercentage);
        }

        /// <summary>
        /// Retry an action using the values provided in the constructor.
        /// </summary>
        /// <param name="actionToTry">Any action.</param>
        public void Action(Action actionToTry)
        {
            Retry.Action(actionToTry, maximumNumberOfAttempts, millisecondsPerSlot, truncateNumberOfSlots, maximumNumberOfSlotsWhenTruncated, jitterPercentage);
        }
    }
}