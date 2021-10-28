using AutoFixture;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Gateways;
using EqualityInformationApi.V1.Infrastructure;
using FluentAssertions;
using Hackney.Core.Testing.DynamoDb;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace EqualityInformationApi.Tests.V1.Gateways
{
    [Collection("Aws collection")]
    public class DynamoDbGatewayTests : IDisposable
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly EqualityInformationGateway _classUnderTest;
        private readonly List<Action> _cleanup = new List<Action>();
        private readonly Mock<ILogger<EqualityInformationGateway>> _logger;
        private readonly IDynamoDbFixture _dbFixture;

        public DynamoDbGatewayTests(MockWebApplicationFactory<Startup> startupFixture)
        {
            _logger = new Mock<ILogger<EqualityInformationGateway>>();
            _dbFixture = startupFixture.DynamoDbFixture;

            _classUnderTest = new EqualityInformationGateway(_dbFixture.DynamoDbContext, _logger.Object);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                foreach (var action in _cleanup)
                    action();

                _disposed = true;
            }
        }

        [Fact]
        public async Task CreateWhenCalledSavesEntityToDatabase()
        {
            // Arrange
            var request = _fixture.Create<EqualityInformationObject>();

            // Act
            var response = await _classUnderTest.Create(request).ConfigureAwait(false);

            // Assert
            response.Should().BeEquivalentTo(request);

            var databaseResponse = await _dbFixture.DynamoDbContext.LoadAsync<EqualityInformationDb>(request.TargetId, response.Id)
                                                                   .ConfigureAwait(false);
            databaseResponse.Should().BeEquivalentTo(request);
        }
    }
}
