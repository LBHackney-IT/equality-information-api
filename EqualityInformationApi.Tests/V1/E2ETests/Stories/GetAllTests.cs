using EqualityInformationApi.Tests.V1.E2ETests.Fixtures;
using EqualityInformationApi.Tests.V1.E2ETests.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestStack.BDDfy;
using Xunit;

namespace EqualityInformationApi.Tests.V1.E2ETests.Stories
{
    [Story(
        AsA = "Service",
        IWant = "an endpoint to return person details",
        SoThat = "it is possible to view the details of a person")]
    [Collection("Aws collection")]
    public class GetAllTests : IDisposable
    {
        private readonly DynamoDbIntegrationTests<Startup> _dbFixture;
        private readonly EqualityInformationFixture _fixture;
        private readonly GetAllSteps _steps;
        private readonly Random _random = new Random();

        public GetAllTests(DynamoDbIntegrationTests<Startup> dbFixture)
        {
            _dbFixture = dbFixture;
            _fixture = new EqualityInformationFixture(_dbFixture.DynamoDbContext);
            _steps = new GetAllSteps(_dbFixture.Client);
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
                if (null != _fixture)
                {
                    _fixture.Dispose();
                }

                _disposed = true;
            }
        }


        [Fact]
        public void ServiceReturnsOkWhenNoEntitiesExist()
        {
            var targetId = Guid.NewGuid();

            this.Given(g => _fixture.GivenAnEntityDoesNotExist())
                .When(w => _steps.WhenTheApiIsCalled(targetId.ToString()))
                .Then(t => _steps.ThenOkResponseIsReturned())
                .And(t => _steps.AndNoEntitiesReturned())
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsOkWhenManyEntitiesExist()
        {
            var targetId = Guid.NewGuid();
            var numberOfEntities = _random.Next(2, 5);

            this.Given(g => _fixture.GivenManyEntitiesExist(numberOfEntities, targetId))
                .When(w => _steps.WhenTheApiIsCalled(targetId.ToString()))
                .Then(t => _steps.ThenOkResponseIsReturned())
                .And(t => _steps.AndManyEntitiesReturned(numberOfEntities))
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnBadRequestIfTargetIdInvalid()
        {
            var targetId = "aaa";

            this.Given(g => _fixture.GivenAnEntityDoesNotExist())
                .When(w => _steps.WhenTheApiIsCalled(targetId))
                .Then(t => _steps.ThenBadRequestIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnBadRequestIfTargetIdEmpty()
        {
            var targetId = Guid.Empty;

            this.Given(g => _fixture.GivenAnEntityDoesNotExist())
                .When(w => _steps.WhenTheApiIsCalled(targetId.ToString()))
                .Then(t => _steps.ThenBadRequestIsReturned())
                .BDDfy();
        }
    }
}
