using System;
using System.Collections.Generic;

namespace RetryMagic
{
    /// <summary>
    /// A custom exception wrapping all internal exceptions
    /// </summary>
    public class RetryException:AggregateException
    {
        public RetryException(string message, IEnumerable<Exception> innerExceptions)
            :base(message,innerExceptions)
        {
        }
    }
}

