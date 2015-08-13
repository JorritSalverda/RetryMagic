using System;
using Moq;
using Xunit;

namespace RetryMagic.UnitTests
{
    public class RetryTests
    {
        public class FunctionMethod
        {
            [Fact]
            public void Returns_Result_If_Function_Succeeds_At_The_First_Attempt()
            {
                var functionToTryMock = new Mock<Func<string>>();
                functionToTryMock.Setup(x => x()).Returns("value");

                // act
                var value = Retry.Function(functionToTryMock.Object);

                Assert.Equal("value", value);
                functionToTryMock.Verify(x => x(), Times.Once);
            }

            [Fact]
            public void Returns_Result_If_Function_Fails_At_The_First_Attempt_But_Succeeds_In_Less_Than_Maximum_Attempts()
            {
                var functionToTryMock = new Mock<Func<string>>();
                var calls = 0;
                functionToTryMock.Setup(x => x()).Returns("value")
                    .Callback(() => {
                    calls++;
                    if (calls == 1)
                    {
                        throw new Exception();
                    }
                });

                // act
                string value = Retry.Function(functionToTryMock.Object);

                Assert.Equal("value", value);
                functionToTryMock.Verify(x => x(), Times.Exactly(2));
            }

            [Fact]
            public void Throws_Aggregate_Exception_If_Function_Fails_For_Maximum_Attempts()
            {
                var functionToTryMock = new Mock<Func<string>>();
                functionToTryMock.Setup(x => x()).Throws<Exception>();

                // act + assert
                Assert.Throws<AggregateException>(() => Retry.Function(functionToTryMock.Object));

                functionToTryMock.Verify(x => x(), Times.Exactly(8));
            }

            [Fact]
            public void Throws_Aggregate_Exception_With_Each_Thrown_Exception_As_Inner_Exception_If_Function_Fails_For_Maximum_Attempts()
            {
                var functionToTryMock = new Mock<Func<string>>();
                functionToTryMock.Setup(x => x()).Throws<Exception>();

                // act + assert
                var aggregateException = Assert.Throws<AggregateException>(() => Retry.Function(functionToTryMock.Object));

                Assert.Equal(8, aggregateException.InnerExceptions.Count);
            }

            [Fact]
            public void Throws_ArgumentOutOfRangeException_If_MaximumNumberOfAttempts_Is_Less_Than_One()
            {
                var functionToTryMock = new Mock<Func<string>>();

                // act + assert
                Assert.Throws<ArgumentOutOfRangeException>(() => Retry.Function(functionToTryMock.Object, 0, 32, true, 16, 25));
            }

            [Fact]
            public void Throws_ArgumentOutOfRangeException_If_MilliSecondsPerSlot_Is_Less_Than_One()
            {
                var functionToTryMock = new Mock<Func<string>>();

                // act + assert
                Assert.Throws<ArgumentOutOfRangeException>(() => Retry.Function(functionToTryMock.Object, 8, 0, true, 16, 25));
            }

            [Fact]
            public void Throws_ArgumentOutOfRangeException_If_MaximumNumberOfSlotsWhenTruncated_Is_Less_Than_One()
            {
                var functionToTryMock = new Mock<Func<string>>();

                // act + assert
                Assert.Throws<ArgumentOutOfRangeException>(() => Retry.Function(functionToTryMock.Object, 8, 32, true, 0, 25));
            }

            [Fact]
            public void Throws_ArgumentOutOfRangeException_If_JitterPercentage_Is_Less_Than_Zero()
            {
                var functionToTryMock = new Mock<Func<string>>();

                // act + assert
                Assert.Throws<ArgumentOutOfRangeException>(() => Retry.Function(functionToTryMock.Object, 8, 32, true, 16, -1));
            }
        }

        public class ActionMethod
        {
            [Fact]
            public void Returns_Result_If_Function_Succeeds_At_The_First_Attempt()
            {
                var actionToTryMock = new Mock<Action>();
                actionToTryMock.Setup(x => x());

                // act
                Retry.Action(actionToTryMock.Object);

                actionToTryMock.Verify(x => x(), Times.Once);
            }

            [Fact]
            public void Returns_Result_If_Function_Fails_At_The_First_Attempt_But_Succeeds_In_Less_Than_Maximum_Attempts()
            {
                var actionToTryMock = new Mock<Action>();
                var calls = 0;
                actionToTryMock.Setup(x => x())
                    .Callback(() => {
                    calls++;
                    if (calls == 1)
                    {
                        throw new Exception();
                    }
                });

                // act
                Retry.Action(actionToTryMock.Object);

                actionToTryMock.Verify(x => x(), Times.Exactly(2));
            }

            [Fact]
            public void Throws_Aggregate_Exception_If_Function_Fails_For_Maximum_Attempts()
            {
                var actionToTryMock = new Mock<Action>();
                actionToTryMock.Setup(x => x()).Throws<Exception>();

                // act + assert
                Assert.Throws<AggregateException>(() => Retry.Action(actionToTryMock.Object));

                actionToTryMock.Verify(x => x(), Times.Exactly(8));
            }

            [Fact]
            public void Throws_Aggregate_Exception_With_Each_Thrown_Exception_As_Inner_Exception_If_Function_Fails_For_Maximum_Attempts()
            {
                var actionToTryMock = new Mock<Action>();
                actionToTryMock.Setup(x => x()).Throws<Exception>();

                // act + assert
                var aggregateException = Assert.Throws<AggregateException>(() => Retry.Action(actionToTryMock.Object));

                Assert.Equal(8, aggregateException.InnerExceptions.Count);
            }

            [Fact]
            public void Throws_ArgumentOutOfRangeException_If_MaximumNumberOfAttempts_Is_Less_Than_One()
            {
                var actionToTryMock = new Mock<Action>();

                // act + assert
                Assert.Throws<ArgumentOutOfRangeException>(() => Retry.Action(actionToTryMock.Object, 0, 32, true, 16, 25));
            }

            [Fact]
            public void Throws_ArgumentOutOfRangeException_If_MilliSecondsPerSlot_Is_Less_Than_One()
            {
                var actionToTryMock = new Mock<Action>();

                // act + assert
                Assert.Throws<ArgumentOutOfRangeException>(() => Retry.Action(actionToTryMock.Object, 8, 0, true, 16, 25));
            }

            [Fact]
            public void Throws_ArgumentOutOfRangeException_If_MaximumNumberOfSlotsWhenTruncated_Is_Less_Than_One()
            {
                var actionToTryMock = new Mock<Action>();

                // act + assert
                Assert.Throws<ArgumentOutOfRangeException>(() => Retry.Action(actionToTryMock.Object, 8, 32, true, 0, 25));
            }

            [Fact]
            public void Throws_ArgumentOutOfRangeException_If_JitterPercentage_Is_Less_Than_Zero()
            {
                var actionToTryMock = new Mock<Action>();

                // act + assert
                Assert.Throws<ArgumentOutOfRangeException>(() => Retry.Action(actionToTryMock.Object, 8, 32, true, 16, -1));
            }
        }
    }
}
