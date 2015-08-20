using System;
using JitterMagic;

namespace RetryMagic
{
    public class RetrySettings
    {
        /// <summary>
        /// Determines the maximum number of attempts to retry for.
        /// </summary>
        public int MaximumNumberOfAttempts { get; private set; }

        /// <summary>
        /// With each attempt the amount of slots doubles; this is the time per slot.
        /// </summary>
        public int MillisecondsPerSlot { get; private set; }

        /// <summary>
        /// Determines whether the exponential backoff truncates or continuous to expand.
        /// </summary>
        public bool TruncateNumberOfSlots { get; private set; }

        /// <summary>
        /// If <see cref="Truncate"/> is true this maximizes the number of slots.
        /// </summary>
        public int MaximumNumberOfSlotsWhenTruncated { get; private set; }

        /// <summary>
        /// Gets or sets the jitter settings used for applying jitter to the retry interval.
        /// </summary>
        /// <value>The jitter percentage.</value>
        public JitterSettings JitterSettings { get; set; }

        public RetrySettings(int maximumNumberOfAttempts = 5, int millisecondsPerSlot = 32, bool truncateNumberOfSlots = true, int maximumNumberOfSlotsWhenTruncated = 16)
            :this(new JitterSettings(), maximumNumberOfAttempts, millisecondsPerSlot, truncateNumberOfSlots, maximumNumberOfSlotsWhenTruncated)
        {
        }

        public RetrySettings(JitterSettings jitterSettings, int maximumNumberOfAttempts = 5, int millisecondsPerSlot = 32, bool truncateNumberOfSlots = true, int maximumNumberOfSlotsWhenTruncated = 16)
        {
            MaximumNumberOfAttempts = maximumNumberOfAttempts;
            MillisecondsPerSlot = millisecondsPerSlot;
            TruncateNumberOfSlots = truncateNumberOfSlots;
            MaximumNumberOfSlotsWhenTruncated = maximumNumberOfSlotsWhenTruncated;
            JitterSettings = jitterSettings;

            Validate();
        }

        /// <summary>
        /// Validate this instance.
        /// </summary>
        internal void Validate()
        {
            if (JitterSettings == null)
            {
                throw new ArgumentNullException("jitterSettings");
            }
            if (MaximumNumberOfAttempts < 1)
            {
                throw new ArgumentOutOfRangeException("maximumNumberOfAttempts", "Maximum number of attempts needs to be 1 or larger");
            }
            if (MillisecondsPerSlot < 1)
            {
                throw new ArgumentOutOfRangeException("milliSecondsPerSlot", "Milliseconds per slot needs to be 1 or larger");
            }
            if (MaximumNumberOfSlotsWhenTruncated < 1)
            {
                throw new ArgumentOutOfRangeException("maximumNumberOfSlotsWhenTruncated", "Maximum number of slots when truncated needs to be 1 or larger");
            }
        }
    }
}

