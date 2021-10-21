using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
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
        private DynamoDbGateway _classUnderTest;
        private readonly List<Action> _cleanup = new List<Action>();
        private Mock<ILogger<DynamoDbGateway>> _logger;
        private readonly IDynamoDBContext _dynamoDb;

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

        public DynamoDbGatewayTests(DynamoDbIntegrationTests<Startup> dbTestFixture)
        {
            _logger = new Mock<ILogger<DynamoDbGateway>>();
            _dynamoDb = dbTestFixture.DynamoDbContext;

            _classUnderTest = new DynamoDbGateway(_dynamoDb, _logger.Object);
        }

        // [Fact]
        // public async Task GetEntityByIdReturnsNullIfEntityDoesntExist()
        // {
        //     var response = await _classUnderTest.GetEntityById(123).ConfigureAwait(false);

        //     response.Should().BeNull();
        //     _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.LoadAsync for id parameter 123", Times.Once());

        // }

        // [Fact]
        // public async Task VerifiesGatewayMethodsAddtoDB()
        // {
        //     var entity = _fixture.Build<DatabaseEntity>()
        //                            .With(x => x.CreatedAt, DateTime.UtcNow).Create();
        //     InsertDatatoDynamoDB(entity);

        //     var result = await _classUnderTest.GetEntityById(entity.Id).ConfigureAwait(false);
        //     result.Should().BeEquivalentTo(entity);
        //     _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.LoadAsync for id parameter {entity.Id}", Times.Once());
        // }

        private void InsertDatatoDynamoDB(DatabaseEntity entity)
        {
            _dynamoDb.SaveAsync<DatabaseEntity>(entity).GetAwaiter().GetResult();
            _cleanup.Add(async () => await _dynamoDb.DeleteAsync(entity).ConfigureAwait(false));
        }
    }
}
