using MicroCQRS.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Reflection;

namespace MicroCQRS.Tests
{
    public class RegistryTests
    {
        private Mock<IServiceCollection> _serviceCollectionMock;
        private Assembly _assembly;

        [SetUp]
        public void Setup()
        {
            _serviceCollectionMock = new Mock<IServiceCollection>();
            _assembly = Assembly.GetExecutingAssembly();
        }

        [Test]
        public void AddMicroCQRS_ShouldRegisterCommandAndQueryHandlersAndDispatcher()
        {
            // Act
            Registry.AddMicroCQRS(_serviceCollectionMock.Object, _assembly);

            // Assert
            _serviceCollectionMock.Verify(sc => sc.Add(It.Is<ServiceDescriptor>(sd =>
                sd.ServiceType.IsGenericType &&
                sd.ServiceType.GetGenericTypeDefinition() == typeof(ICommandHandler<>))), Times.AtLeastOnce);

            _serviceCollectionMock.Verify(sc => sc.Add(It.Is<ServiceDescriptor>(sd =>
                sd.ServiceType.IsGenericType &&
                sd.ServiceType.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))), Times.AtLeastOnce);

            _serviceCollectionMock.Verify(sc => sc.Add(It.Is<ServiceDescriptor>(sd =>
                sd.ServiceType == typeof(IDispatcher) &&
                sd.Lifetime == ServiceLifetime.Scoped)), Times.Once);
        }

        [Test]
        [TestCase(typeof(ICommandHandler<>), 2)]
        [TestCase(typeof(IQueryHandler<,>), 1)]
        public void AddGenericTypes_ShouldRegisterHandlers(Type type, int count)
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddMicroCQRS(_assembly);

            // Assert
            var registeredServices = serviceCollection.Where(sd => sd.ServiceType.IsGenericType &&
                sd.ServiceType.GetGenericTypeDefinition() == type).ToList();

            Assert.That(registeredServices.Count, Is.EqualTo(count));
            foreach (var serviceDescriptor in registeredServices)
            {
                Assert.That(serviceDescriptor.Lifetime, Is.EqualTo(ServiceLifetime.Transient));
            }
        }
    }
}
