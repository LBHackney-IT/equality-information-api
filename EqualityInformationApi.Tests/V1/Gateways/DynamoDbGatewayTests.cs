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
        private readonly Mock<IEntityUpdater> _mockUpdater;
        private readonly Random _random = new Random();

        public DynamoDbGatewayTests(DynamoDbIntegrationTests<Startup> dbTestFixture)
        {
            _logger = new Mock<ILogger<EqualityInformationGateway>>();
            _dynamoDb = dbTestFixture.DynamoDbContext;

            _mockUpdater = new Mock<IEntityUpdater>();

            _classUnderTest = new EqualityInformationGateway(_dynamoDb, _mockUpdater.Object, _logger.Object);
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
        public async Task GetByIdWhenEntityDoesntExistReturnsNull()
        {
            // Arrange
            var id = Guid.NewGuid();
            var targetId = Guid.NewGuid();

            // Act
            var response = await _classUnderTest.GetById(id, targetId).ConfigureAwait(false);

            // Assert
            response.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdWhenEntityExistsReturnsEntity()
        {
            // Arrange
            var entity = _fixture.Create<EqualityInformationDb>();
            await InsertDatatoDynamoDB(entity).ConfigureAwait(false);

            // Act
            var response = await _classUnderTest.GetById(entity.Id, entity.TargetId).ConfigureAwait(false);

            // Assert
            response.Should().BeEquivalentTo(entity);
        }

        [Fact]
        public async Task GetAllWhenNoEntitesExistReturnsEmptyList()
        {
            // Arrange
            var targetId = Guid.NewGuid();

            // Act
            var response = await _classUnderTest.GetAll(targetId).ConfigureAwait(false);

            // Assert
            response.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetAllWhenManyExistReturnsMany()
        {
            // Arrange
            var numberOfEntities = _random.Next(2, 5);
            var targetId = Guid.NewGuid();

            var entities = _fixture.Build<EqualityInformationDb>()
                .With(x => x.TargetId, targetId)
                .CreateMany(numberOfEntities);

            await InsertDatatoDynamoDB(entities).ConfigureAwait(false);

            // Act
            var response = await _classUnderTest.GetAll(targetId).ConfigureAwait(false);

            // Assert
            response.Should().HaveCount(numberOfEntities);
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

        [Fact]
        public async Task UpdateWhenEntityDoesntExistReturnsNull()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = _fixture.Create<EqualityInformationObject>();
            var requestBody = "";

            // Act
            var response = await _classUnderTest.Update(id, request, requestBody).ConfigureAwait(false);

            // Assert
            response.Should().BeNull();
        }

        [Fact]
        public async Task UpdateWhenNoChangesInRequestDoesntUpdateDatabase()
        {
            // Arrange
            var request = _fixture.Create<EqualityInformationObject>();
            var requestBody = "";

            var entity = _fixture.Build<EqualityInformationDb>()
                .With(x => x.TargetId, request.TargetId)
                .Create();

            await InsertDatatoDynamoDB(entity).ConfigureAwait(false);

            var updaterResponse = new UpdateEntityResult<EqualityInformationDb>
            {
                NewValues = new Dictionary<string, object>(), // empty - no changes
            };

            _mockUpdater
                .Setup(x => x.UpdateEntity(It.IsAny<EqualityInformationDb>(), It.IsAny<string>(), It.IsAny<EqualityInformationObject>()))
                .Returns(updaterResponse);

            // Act
            var response = await _classUnderTest.Update(entity.Id, request, requestBody).ConfigureAwait(false);

            // Assert
            var databaseResponse = await _dynamoDb.LoadAsync<EqualityInformationDb>(request.TargetId, entity.Id).ConfigureAwait(false);
            // be same as original entity, with no changes
            databaseResponse.Should().BeEquivalentTo(entity);
        }

        [Fact]
        public async Task UpdateWhenRequestHasChangesUpdatesDatabase()
        {
            // Arrange
            var request = _fixture.Create<EqualityInformationObject>();
            var requestBody = "";

            var entity = _fixture.Build<EqualityInformationDb>()
                .With(x => x.TargetId, request.TargetId)
                .Create();

            await InsertDatatoDynamoDB(entity).ConfigureAwait(false);

            var updaterNewEntity = _fixture.Build<EqualityInformationDb>()
                .With(x => x.TargetId, request.TargetId)
                .With(x => x.Id, entity.Id)
                .Create();

            var updaterResponse = new UpdateEntityResult<EqualityInformationDb>
            {
                NewValues = new Dictionary<string, object>() { { "key", "value" } },
                UpdatedEntity = updaterNewEntity
            };

            _mockUpdater
                .Setup(x => x.UpdateEntity(It.IsAny<EqualityInformationDb>(), It.IsAny<string>(), It.IsAny<EqualityInformationObject>()))
                .Returns(updaterResponse);

            // Act
            var response = await _classUnderTest.Update(entity.Id, request, requestBody).ConfigureAwait(false);

            // Assert
            response.Should().BeEquivalentTo(updaterNewEntity);

            var databaseResponse = await _dynamoDb.LoadAsync<EqualityInformationDb>(request.TargetId, entity.Id).ConfigureAwait(false);
            // be same as original entity, with no changes
            databaseResponse.Should().BeEquivalentTo(updaterNewEntity);
        }

        private async Task InsertDatatoDynamoDB(IEnumerable<EqualityInformationDb> entities)
        {
            foreach (var entity in entities)
            {
                await InsertDatatoDynamoDB(entity).ConfigureAwait(false);
            }
        }

        private async Task InsertDatatoDynamoDB(EqualityInformationDb entity)
        {
            await _dynamoDb.SaveAsync(entity).ConfigureAwait(false);
            _cleanup.Add(async () => await _dynamoDb.DeleteAsync(entity).ConfigureAwait(false));
        }
    }
}
