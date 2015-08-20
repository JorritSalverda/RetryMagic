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
            public void Throws_ArgumentNullException_If_Settings_Is_Null()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => new RetryInstance(null));
            }
        }

        public class UpdateSettings
        {
            private readonly IRetryInstance instance;

            public UpdateSettings()
            {
                instance = new RetryInstance(new RetrySettings());
            }

            [Fact]
            public void Throws_ArgumentNullException_If_Settings_Is_Null()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => instance.UpdateSettings(null));
            }

            [Fact]
            public void Updates_Settings_Property()
            {
                // act
                instance.UpdateSettings(new RetrySettings(maximumNumberOfAttempts: 10));

                Assert.Equal(10, instance.Settings.MaximumNumberOfAttempts);
            }
        }

        public class FunctionMethod
        {
            private readonly IRetryInstance instance;

            public FunctionMethod()
            {
                instance = new RetryInstance(new RetrySettings(maximumNumberOfAttempts:5));
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
                    .Callback(() => {
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

                Assert.Equal(5, aggregateException.InnerExceptions.Count);
            }


        }

        public class ActionMethod
        {
            private readonly IRetryInstance instance;

            public ActionMethod()
            {
                instance = new RetryInstance(new RetrySettings(maximumNumberOfAttempts:5));
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
                    .Callback(() => {
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

                actionToTryMock.Verify(x => x(), Times.Exactly(5));
            }

            [Fact]
            public void Throws_Aggregate_Exception_With_Each_Thrown_Exception_As_Inner_Exception_If_Function_Fails_For_Maximum_Attempts()
            {
                var actionToTryMock = new Mock<Action>();
                actionToTryMock.Setup(x => x()).Throws<Exception>();

                // act + assert
                var aggregateException = Assert.Throws<AggregateException>(() => instance.Action(actionToTryMock.Object));

                Assert.Equal(5, aggregateException.InnerExceptions.Count);
            }
        }
    }
}