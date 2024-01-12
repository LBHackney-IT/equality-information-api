using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Factories;
using EqualityInformationApi.V1.Gateways;
using EqualityInformationApi.V1.Infrastructure;
using EqualityInformationApi.V1.Infrastructure.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
using Hackney.Core.DynamoDb.EntityUpdater;
using Hackney.Core.DynamoDb.EntityUpdater.Interfaces;
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
        private readonly Mock<IEntityUpdater> _mockUpdater;

        public DynamoDbGatewayTests(MockWebApplicationFactory<Startup> startupFixture)
        {
            _logger = new Mock<ILogger<EqualityInformationGateway>>();
            _dbFixture = startupFixture.DynamoDbFixture;
            _mockUpdater = new Mock<IEntityUpdater>();

            _classUnderTest = new EqualityInformationGateway(_dbFixture.DynamoDbContext, _mockUpdater.Object, _logger.Object);
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

        [Fact]
        public void CreateWhenCalledThrowsException()
        {
            // Arrange
            var request = _fixture.Create<EqualityInformationObject>();
            var exception = new ApplicationException("An error");
            var mockDbContext = new Mock<IDynamoDBContext>();
            mockDbContext.Setup(x => x.SaveAsync(It.IsAny<EqualityInformationDb>(), default)).ThrowsAsync(exception);

            var classUnderTest = new EqualityInformationGateway(mockDbContext.Object, _mockUpdater.Object, _logger.Object);

            // Act
            Func<Task<EqualityInformation>> func = async () => await classUnderTest.Create(request).ConfigureAwait(false);

            // Assert
            func.Should().ThrowAsync<ApplicationException>().WithMessage(exception.Message);
        }

        [Fact]
        public async Task GetWhenCalledReturnsEntity()
        {
            // Arrange
            var dbEntity = _fixture.Build<EqualityInformationDb>()
                                   .With(x => x.VersionNumber, (int?) null)
                                   .Create();
            await _dbFixture.SaveEntityAsync(dbEntity).ConfigureAwait(false);

            // Act
            var response = await _classUnderTest.Get(dbEntity.TargetId).ConfigureAwait(false);

            // Assert
            response.Should().BeEquivalentTo(dbEntity.ToDomain());
        }

        [Fact]
        public void GetWhenCalledThrowsException()
        {
            // Arrange
            var exception = new ApplicationException("An error");
            var mockDbContext = new Mock<IDynamoDBContext>();
            var id = Guid.NewGuid();
            mockDbContext.Setup(x => x.QueryAsync<EqualityInformationDb>(It.IsAny<object>(), It.IsAny<DynamoDBOperationConfig>()))
                         .Throws(exception);

            var classUnderTest = new EqualityInformationGateway(mockDbContext.Object, _mockUpdater.Object, _logger.Object);

            // Act
            Func<Task<EqualityInformation>> func = async () => await classUnderTest.Get(id).ConfigureAwait(false);

            // Assert
            func.Should().ThrowAsync<ApplicationException>().WithMessage(exception.Message);
        }

        [Fact]
        public async Task GetWhenCalledGatewayReturnsNoResultsReturnsNull()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var result = await _classUnderTest.Get(id).ConfigureAwait(false);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetWhenCalledReturnsResult()
        {
            // Arrange
            var dbEntity = _fixture.Build<EqualityInformationDb>()
                                   .With(x => x.VersionNumber, (int?) null)
                                   .Create();
            await _dbFixture.SaveEntityAsync(dbEntity).ConfigureAwait(false);

            // Act
            var result = await _classUnderTest.Get(dbEntity.TargetId).ConfigureAwait(false);

            // Assert
            result.Should().BeEquivalentTo(dbEntity.ToDomain());
        }

        [Fact]
        public async Task GetWhenCalledGatewayReturnsManyResult()
        {
            // Arrange
            int count = 3;
            var targetId = Guid.NewGuid();
            var dbEntities = _fixture.Build<EqualityInformationDb>()
                                     .With(x => x.TargetId, targetId)
                                     .With(x => x.VersionNumber, (int?) null)
                                     .CreateMany(count);
            foreach (var dbEntity in dbEntities)
                await _dbFixture.SaveEntityAsync(dbEntity).ConfigureAwait(false);

            // Act
            Func<Task<EqualityInformation>> func = async () =>
                await _classUnderTest.Get(targetId).ConfigureAwait(false);

            // Assert
            await func.Should().ThrowAsync<ApplicationException>()
                               .WithMessage($"{count} EqualityInformationDb records found for target id {targetId}. "
                                           + "There should only be 0 or 1.");
        }

        [Fact]
        public async Task UpdateWhenCalledGatewayReturnsNoResultReturnsNull()
        {
            // Arrange
            var requestObject = _fixture.Create<EqualityInformationObject>();
            var request = _fixture.Create<PatchEqualityInformationRequest>();

            // Act
            var result = await _classUnderTest.Update(request, requestObject, null, null).ConfigureAwait(false);

            // Assert
            result.Should().BeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(5)]
        public async Task UpdateWhenCalledThrowsExceptionOnVersionConflict(int? ifMatch)
        {
            // Arrange
            var dbEntity = _fixture.Build<EqualityInformationDb>()
                                   .With(x => x.VersionNumber, (int?) null)
                                   .Create();
            await _dbFixture.SaveEntityAsync(dbEntity).ConfigureAwait(false);

            var request = new PatchEqualityInformationRequest() { Id = dbEntity.Id };
            var requestObject = _fixture.Build<EqualityInformationObject>()
                                  .With(x => x.TargetId, dbEntity.TargetId)
                                  .Create();

            // Act
            Func<Task> act = async () =>
            {
                await _classUnderTest.Update(request, requestObject, null, ifMatch).ConfigureAwait(false);
            };

            // Assert
            act.Should().Throw<VersionNumberConflictException>()
               .Where(x => (x.IncomingVersionNumber == ifMatch) && (x.ExpectedVersionNumber == 0));
        }

        [Fact]
        public async Task UpdateWhenCalledUpdatesRecordInDatabase()
        {
            // Arrange
            var dbEntity = _fixture.Build<EqualityInformationDb>()
                                   .With(x => x.VersionNumber, (int?) null)
                                   .Create();
            await _dbFixture.SaveEntityAsync(dbEntity).ConfigureAwait(false);

            var request = new PatchEqualityInformationRequest() { Id = dbEntity.Id };
            var requestObject = new EqualityInformationObject()
            {
                TargetId = dbEntity.TargetId,
                Disabled = "might be",
                Ethnicity = new Ethnicity() { EthnicGroupValue = "some-ethnic-group" },
                ReligionOrBelief = new ReligionOrBelief() { ReligionOrBeliefValue = "its all rubbish" }
            };

            // setup updater
            var updaterResponse = CreateUpdateEntityResultWithChanges(dbEntity, requestObject);
            _mockUpdater
                .Setup(x => x.UpdateEntity(It.IsAny<EqualityInformationDb>(), It.IsAny<string>(), It.IsAny<EqualityInformationObject>()))
                .Returns(updaterResponse);

            var mockRawBody = "";

            // Act
            var result = await _classUnderTest.Update(request, requestObject, mockRawBody, 0).ConfigureAwait(false);

            // Assert
            result.Should().BeOfType(typeof(UpdateEntityResult<EqualityInformationDb>));

            var updatedInDb = await _dbFixture.DynamoDbContext.LoadAsync<EqualityInformationDb>(dbEntity.TargetId, dbEntity.Id)
                                                                .ConfigureAwait(false);
            updatedInDb.Should().BeEquivalentTo(dbEntity, c => c.Excluding(y => y.VersionNumber)
                                                                .Excluding(y => y.Disabled)
                                                                .Excluding(y => y.Ethnicity)
                                                                .Excluding(y => y.ReligionOrBelief));
            updatedInDb.VersionNumber.Should().Be(1);
            updatedInDb.Disabled.Should().Be(requestObject.Disabled);
            updatedInDb.Ethnicity.Should().BeEquivalentTo(requestObject.Ethnicity);
            updatedInDb.ReligionOrBelief.Should().BeEquivalentTo(requestObject.ReligionOrBelief);
        }

        [Fact]
        public async Task UpdateWhenCalledWithNoChangesDoesNotUpdateDatabase()
        {
            // Arrange
            var dbEntity = _fixture.Build<EqualityInformationDb>()
                                   .With(x => x.VersionNumber, (int?) null)
                                   .Create();
            await _dbFixture.SaveEntityAsync(dbEntity).ConfigureAwait(false);

            var request = new PatchEqualityInformationRequest() { Id = dbEntity.Id };
            var requestObject = new EqualityInformationObject() { TargetId = dbEntity.TargetId };

            // setup updater
            var updaterResponse = new UpdateEntityResult<EqualityInformationDb>(); // no changes
            _mockUpdater
                .Setup(x => x.UpdateEntity(It.IsAny<EqualityInformationDb>(), It.IsAny<string>(), It.IsAny<EqualityInformationObject>()))
                .Returns(updaterResponse);

            var mockRawBody = "";

            // Act
            var result = await _classUnderTest.Update(request, requestObject, mockRawBody, 0).ConfigureAwait(false);

            // Assert
            result.Should().BeOfType(typeof(UpdateEntityResult<EqualityInformationDb>));

            var loadedInDb = await _dbFixture.DynamoDbContext.LoadAsync<EqualityInformationDb>(dbEntity.TargetId, dbEntity.Id)
                                                                .ConfigureAwait(false);
            loadedInDb.Should().BeEquivalentTo(dbEntity, c => c.Excluding(y => y.VersionNumber));
            loadedInDb.VersionNumber.Should().Be(0);
        }

        private UpdateEntityResult<EqualityInformationDb> CreateUpdateEntityResultWithChanges(EqualityInformationDb entityInsertedIntoDatabase,
            EqualityInformationObject request)
        {
            var updatedEntity = entityInsertedIntoDatabase.DeepClone();
            updatedEntity.Disabled = request.Disabled;
            updatedEntity.Ethnicity = request.Ethnicity;
            updatedEntity.ReligionOrBelief = request.ReligionOrBelief;

            return new UpdateEntityResult<EqualityInformationDb>
            {
                UpdatedEntity = updatedEntity,
                OldValues = new Dictionary<string, object>
                {
                     { "Disabled", entityInsertedIntoDatabase.Disabled },
                     { "Ethnicity", entityInsertedIntoDatabase.Ethnicity },
                     { "ReligionOrBelief", entityInsertedIntoDatabase.ReligionOrBelief }
                },
                NewValues = new Dictionary<string, object>
                {
                     { "Disabled", updatedEntity.Disabled },
                     { "Ethnicity", updatedEntity.Ethnicity },
                     { "ReligionOrBelief", updatedEntity.ReligionOrBelief }
                }
            };
        }
    }
}
