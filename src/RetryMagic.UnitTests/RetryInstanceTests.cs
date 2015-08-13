using System;
using Moq;
using Xunit;

namespace RetryMagic.UnitTests
{
    public class RetryInstanceTests
    {
        public class Constructor
        {
            [Fact]
            public void Throws_ArgumentOutOfRangeException_If_MaximumNumberOfAttempts_Is_Less_Than_One()
            {

                // act + assert
                Assert.Throws<ArgumentOutOfRangeException>(new RetryInstance(0, 32, true, 16, 25));
            }

            [Fact]
            public void Throws_ArgumentOutOfRangeException_If_MilliSecondsPerSlot_Is_Less_Than_One()
            {
                var functionToTryMock = new Mock<Func<string>>();

                // act + assert
                Assert.Throws<ArgumentOutOfRangeException>(new RetryInstance(8, 0, true, 16, 25));
            }

            [Fact]
            public void Throws_ArgumentOutOfRangeException_If_MaximumNumberOfSlotsWhenTruncated_Is_Less_Than_One()
            {
                var functionToTryMock = new Mock<Func<string>>();

                // act + assert
                Assert.Throws<ArgumentOutOfRangeException>(new RetryInstance(8, 32, true, 0, 25));
            }

            [Fact]
            public void Throws_ArgumentOutOfRangeException_If_JitterPercentage_Is_Less_Than_Zero()
            {
                var functionToTryMock = new Mock<Func<string>>();

                // act + assert
                Assert.Throws<ArgumentOutOfRangeException>(new RetryInstance(8, 32, true, 16, -1));
            }
        }

        public class Function
        {
            private readonly IRetryInstance instance;

            public Function()
            {
                instance = new RetryInstance(8, 32, true, 16, 25);
            }

            [Fact]
            public void Returns_Result_If_Function_Succeeds_At_The_First_Attempt()
            {
                var functionToTryMock = new Mock<Func<string>>();
                functionToTryMock.Setup(x => x()).Returns("value");

                // act
                var value = instance.Function(functionToTryMock.Object);

                Assert.Equal("value", value);
                functionToTryMock.Verify(x => x(), Times.Once);
            }

            [Fact]
            public void Returns_Result_If_Function_Fails_At_The_First_Attempt_But_Succeeds_In_Less_Than_Maximum_Attempts()
            {
                var functionToTryMock = new Mock<Func<string>>();
                var calls = 0;
                functionToTryMock.Setup(x => x()).Returns("value")
                    .Callback(() =>
                        {
                            calls++;
                            if (calls == 1)
                            {
                                throw new Exception();
                            }
                        });

                // act
                string value = instance.Function(functionToTryMock.Object);

                Assert.Equal("value", value);
                functionToTryMock.Verify(x => x(), Times.Exactly(2));
            }

            [Fact]
            public void Throws_Aggregate_Exception_If_Function_Fails_For_Maximum_Attempts()
            {
                var functionToTryMock = new Mock<Func<string>>();
                functionToTryMock.Setup(x => x()).Throws<Exception>();

                // act + assert
                Assert.Throws<AggregateException>(() => instance.Function(functionToTryMock.Object));

                functionToTryMock.Verify(x => x(), Times.Exactly(8));
            }

            [Fact]
            public void Throws_Aggregate_Exception_With_Each_Thrown_Exception_As_Inner_Exception_If_Function_Fails_For_Maximum_Attempts()
            {
                var functionToTryMock = new Mock<Func<string>>();
                functionToTryMock.Setup(x => x()).Throws<Exception>();

                // act + assert
                var aggregateException = Assert.Throws<AggregateException>(() => instance.Function(functionToTryMock.Object));

                Assert.Equal(8, aggregateException.InnerExceptions.Count);
            }


        }

        public class Action
        {
            private readonly IRetryInstance instance;

            public Action()
            {
                instance = new RetryInstance(8, 32, true, 16, 25);
            }

            [Fact]
            public void Returns_Result_If_Function_Succeeds_At_The_First_Attempt()
            {
                var actionToTryMock = new Mock<Action>();
                actionToTryMock.Setup(x => x());

                // act
                instance.Action(actionToTryMock.Object);

                actionToTryMock.Verify(x => x(), Times.Once);
            }

            [Fact]
            public void Returns_Result_If_Function_Fails_At_The_First_Attempt_But_Succeeds_In_Less_Than_Maximum_Attempts()
            {
                var actionToTryMock = new Mock<Action>();
                var calls = 0;
                actionToTryMock.Setup(x => x())
                    .Callback(() =>
                        {
                            calls++;
                            if (calls == 1)
                            {
                                throw new Exception();
                            }
                        });

                // act
                instance.Action(actionToTryMock.Object);

                actionToTryMock.Verify(x => x(), Times.Exactly(2));
            }

            [Fact]
            public void Throws_Aggregate_Exception_If_Function_Fails_For_Maximum_Attempts()
            {
                var actionToTryMock = new Mock<Action>();
                actionToTryMock.Setup(x => x()).Throws<Exception>();

                // act + assert
                Assert.Throws<AggregateException>(() => instance.Action(actionToTryMock.Object));

                actionToTryMock.Verify(x => x(), Times.Exactly(8));
            }

            [Fact]
            public void Throws_Aggregate_Exception_With_Each_Thrown_Exception_As_Inner_Exception_If_Function_Fails_For_Maximum_Attempts()
            {
                var actionToTryMock = new Mock<Action>();
                actionToTryMock.Setup(x => x()).Throws<Exception>();

                // act + assert
                var aggregateException = Assert.Throws<AggregateException>(() => instance.Action(actionToTryMock.Object));

                Assert.Equal(8, aggregateException.InnerExceptions.Count);
            }
        }    
    }
}