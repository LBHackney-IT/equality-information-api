using EqualityInformationApi.Tests.V1.E2ETests.Fixtures;
using EqualityInformationApi.Tests.V1.E2ETests.Steps;
using Hackney.Core.Testing.DynamoDb;
using System;
using TestStack.BDDfy;
using Xunit;

namespace EqualityInformationApi.Tests.V1.E2ETests.Stories
{
    [Story(
        AsA = "Service",
        IWant = "an endpoint to get equality information",
        SoThat = "it is possible to get equality information related to a person")]
    [Collection("Aws collection")]
    public class GetTests : IDisposable
    {
        private readonly IDynamoDbFixture _dbFixture;
        private readonly EqualityInformationFixture _testFixture;
        private readonly GetSteps _steps;

        public GetTests(MockWebApplicationFactory<Startup> startupFixture)
        {
            _dbFixture = startupFixture.DynamoDbFixture;
            _testFixture = new EqualityInformationFixture(_dbFixture, startupFixture.SnsFixture.SimpleNotificationService);
            _steps = new GetSteps(startupFixture.Client);
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
                _testFixture?.Dispose();

                _disposed = true;
            }
        }

        [Fact]
        public void ServiceGetsEntityCorrectly()
        {
            var targetId = Guid.NewGuid();

            this.Given(x => _testFixture.GivenAnEntityExists(targetId, Guid.NewGuid()))
                .When(w => _steps.WhenTheApiIsCalledToGet(targetId))
                .Then(t => _steps.ThenTheEntityIsReturned(_dbFixture.DynamoDbContext))
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsBadRequestWhenTooManyRecords()
        {
            var targetId = Guid.NewGuid();

            this.Given(x => _testFixture.GivenAnEntityExists(targetId, Guid.NewGuid()))
                .And(x => _testFixture.GivenAnEntityExists(targetId, Guid.NewGuid()))
                .When(w => _steps.WhenTheApiIsCalledToGet(targetId))
                .Then(t => _steps.Then500IsReturned())
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsNotFoundWhenTargetIdNotFound()
        {
            var targetId = Guid.NewGuid();

            this.Given(g => _testFixture.GivenAnEntityDoesNotExist())
                .When(w => _steps.WhenTheApiIsCalledToGet(targetId))
                .Then(t => _steps.ThenNotFoundIsReturned())
                .BDDfy();
        }
    }
}
