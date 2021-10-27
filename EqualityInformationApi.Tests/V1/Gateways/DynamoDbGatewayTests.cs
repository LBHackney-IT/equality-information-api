using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Factories;
using EqualityInformationApi.V1.Gateways;
using EqualityInformationApi.V1.Infrastructure;
using FluentAssertions;
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
        private EqualityInformationGateway _classUnderTest;
        private readonly List<Action> _cleanup = new List<Action>();
        private Mock<ILogger<EqualityInformationGateway>> _logger;
        private readonly IDynamoDBContext _dynamoDb;

        public DynamoDbGatewayTests(DynamoDbIntegrationTests<Startup> dbTestFixture)
        {
            _logger = new Mock<ILogger<EqualityInformationGateway>>();
            _dynamoDb = dbTestFixture.DynamoDbContext;

            _classUnderTest = new EqualityInformationGateway(_dynamoDb, _logger.Object);
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

            var databaseResponse = await _dynamoDb.LoadAsync<EqualityInformationDb>(request.TargetId, response.Id).ConfigureAwait(false);
            databaseResponse.Should().BeEquivalentTo(request);
        }

        private async Task InsertDatatoDynamoDB(EqualityInformationDb entity)
        {
            await _dynamoDb.SaveAsync(entity).ConfigureAwait(false);
            _cleanup.Add(async () => await _dynamoDb.DeleteAsync(entity).ConfigureAwait(false));
        }
    }
}
