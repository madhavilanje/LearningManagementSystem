using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using LMS.UserManagement.Api.Extensions;
using LMS.Application.Interfaces;
using LMS.Infrastructure.Common.Interfaces;
using LMS.Infrastructure.Common;
using LMS.Infrastructure.SqlClietInterfaces;
using Microsoft.Extensions.Logging;

namespace LMS.Test.Api.ServiceExtensions
{
    public class ServiceExtensionsTests
    {
        [SetUp]
        public void SetUp()
        {

        }
        [Test]
        public void ConfigureAutoMapper_ShouldRegisterMapperInstance()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.ConfigureAutoMapper();
            var serviceProvider = services.BuildServiceProvider();
            var mapper = serviceProvider.GetService<IMapper>();

            // Assert
            Assert.NotNull(mapper);
            Assert.IsInstanceOf<Mapper>(mapper);
        }

    }
}
