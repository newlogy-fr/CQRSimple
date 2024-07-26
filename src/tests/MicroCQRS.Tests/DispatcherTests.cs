using MicroCQRS.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace MicroCQRS.Tests
{
    public class DispatcherTests
    {
        private Mock<IServiceProvider> _serviceProviderMock;
        private Dispatcher _dispatcher;

        [SetUp]
        public void Setup()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();
            _dispatcher = new Dispatcher(_serviceProviderMock.Object);
        }

        [Test]
        public async Task DispatchAsync_Command_ShouldCallHandleAsync()
        {
            // Arrange
            var command = new Mock<ICommand>().Object;
            var handlerMock = new Mock<ICommandHandler<ICommand>>();
            handlerMock.Setup(h => h.HandleAsync(command)).Returns(Task.CompletedTask);

            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(ICommandHandler<ICommand>)))
                .Returns(handlerMock.Object);

            // Act
            await _dispatcher.DispatchAsync(command);

            // Assert
            handlerMock.Verify(h => h.HandleAsync(command), Times.Once);
        }

        [Test]
        public async Task DispatchAsync_Query_ShouldCallHandleAsyncAndReturnResult()
        {
            // Arrange
            var query = new Mock<IQuery<string>>().Object;
            var expectedResult = "result";
            var handlerMock = new Mock<IQueryHandler<IQuery<string>, string>>();
            handlerMock.Setup(h => h.HandleAsync(query)).ReturnsAsync(expectedResult);

            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(IQueryHandler<IQuery<string>, string>)))
                .Returns(handlerMock.Object);

            // Act
            var result = await _dispatcher.DispatchAsync<IQuery<string>, string>(query);

            // Assert
            handlerMock.Verify(h => h.HandleAsync(query), Times.Once);
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(99.9)]
        [TestCase(1)]
        public async Task HandleQuery_ShouldBeOk(double value)
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMicroCQRS();
            var dispatcher = serviceCollection.BuildServiceProvider().GetService<IDispatcher>();

            // Act
            var result = await dispatcher.DispatchAsync<DoubleTestQuery, List<double>>(new DoubleTestQuery { Prop = value });

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0], Is.EqualTo(value));
        }

        [Test]
        [TestCase(5)]
        [TestCase(1)]
        public void HandleCammand_ShouldBeOk(int value)
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMicroCQRS();
            var dispatcher = serviceCollection.BuildServiceProvider().GetService<IDispatcher>();

            // Act
            Assert.DoesNotThrowAsync(async () => await dispatcher.DispatchAsync<IntTestCommand>(new IntTestCommand { Prop = value }));
        }
    }
}
