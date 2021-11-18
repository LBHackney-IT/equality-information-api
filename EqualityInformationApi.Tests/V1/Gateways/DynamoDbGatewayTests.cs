using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Factories;
using EqualityInformationApi.V1.Gateways;
using EqualityInformationApi.V1.Infrastructure;
using EqualityInformationApi.V1.Infrastructure.Exceptions;
using FluentAssertions;
using Hackney.Core.Testing.DynamoDb;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [Fact]
        public void CreateWhenCalledThrowsException()
        {
            // Arrange
            var request = _fixture.Create<EqualityInformationObject>();
            var exception = new ApplicationException("An error");
            var mockDbContext = new Mock<IDynamoDBContext>();
            mockDbContext.Setup(x => x.SaveAsync(It.IsAny<EqualityInformationDb>(), default)).ThrowsAsync(exception);

            var classUnderTest = new EqualityInformationGateway(mockDbContext.Object, _logger.Object);

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

            var classUnderTest = new EqualityInformationGateway(mockDbContext.Object, _logger.Object);

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
            var request = _fixture.Create<PatchEqualityInformationObject>();

            // Act
            var result = await _classUnderTest.Update(request, null).ConfigureAwait(false);

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

            var request = _fixture.Build<PatchEqualityInformationObject>()
                                  .With(x => x.Id, dbEntity.Id)
                                  .With(x => x.TargetId, dbEntity.TargetId)
                                  .Create();

            // Act
            Func<Task> act = async () =>
            {
                await _classUnderTest.Update(request, ifMatch).ConfigureAwait(false);
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

            var request = _fixture.Build<PatchEqualityInformationObject>()
                                  .With(x => x.Id, dbEntity.Id)
                                  .With(x => x.TargetId, dbEntity.TargetId)
                                  .Create();

            // Act
            var result = await _classUnderTest.Update(request, 0).ConfigureAwait(false);
            
            // Assert
            result.Should().BeEquivalentTo(request.ToDomain(), c => c.Excluding(y => y.VersionNumber));

            var updatedInDb = await _dbFixture.DynamoDbContext.LoadAsync<EqualityInformationDb>(dbEntity.TargetId, dbEntity.Id)
                                                                .ConfigureAwait(false);
            updatedInDb.Should().BeEquivalentTo(request.ToDomain().ToDatabase(), c => c.Excluding(y => y.VersionNumber));
            updatedInDb.VersionNumber.Should().Be(1);
        }
    }
}
