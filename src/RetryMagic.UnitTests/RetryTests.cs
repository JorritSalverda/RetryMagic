using System;
using Moq;
using Xunit;

namespace RetryMagic.UnitTests
{
    public class RetryTests
    {
        public class UpdateSettings
        {
            [Fact]
            public void Throws_ArgumentNullException_If_Settings_Is_Null()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => Retry.UpdateSettings(null));
            }

            [Fact]
            public void Updates_Settings_Property()
            {
                // act
                Retry.UpdateSettings(new RetrySettings(maximumNumberOfAttempts: 10));

                Assert.Equal(10, Retry.Settings.MaximumNumberOfAttempts);
            }
        }

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
            public void Throws_ArgumentNullException_If_Settings_Is_Null()
            {
                var functionToTryMock = new Mock<Func<string>>();

                // act + assert
                Assert.Throws<ArgumentNullException>(() => Retry.Function(functionToTryMock.Object, null));
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
            public void Throws_ArgumentNullException_If_Settings_Is_Null()
            {
                var actionToTryMock = new Mock<Action>();

                // act + assert
                Assert.Throws<ArgumentNullException>(() => Retry.Action(actionToTryMock.Object, null));
            }
        }
    }
}
