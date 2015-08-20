using System;
using Moq;
using Xunit;

namespace RetryMagic.UnitTests
{
    public class RetrySettingsTests
    {
        public class Constructor
        {       
            [Fact]
            public void Throws_ArgumentNullException_If_JitterSettings_Is_Null()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => new RetrySettings(jitterSettings:null));
            }

            [Fact]
            public void Throws_ArgumentOutOfRangeException_If_MaximumNumberOfAttempts_Is_Less_Than_One()
            {
                // act + assert
                Assert.Throws<ArgumentOutOfRangeException>(() => new RetrySettings(maximumNumberOfAttempts:0));
            }

            [Fact]
            public void Throws_ArgumentOutOfRangeException_If_MilliSecondsPerSlot_Is_Less_Than_One()
            {
                // act + assert
                Assert.Throws<ArgumentOutOfRangeException>(() => new RetrySettings(millisecondsPerSlot:0));
            }

            [Fact]
            public void Throws_ArgumentOutOfRangeException_If_MaximumNumberOfSlotsWhenTruncated_Is_Less_Than_One()
            {
                // act + assert
                Assert.Throws<ArgumentOutOfRangeException>(() => new RetrySettings(maximumNumberOfSlotsWhenTruncated:0));
            }
        }
    }
}
