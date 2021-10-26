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
        IWant = "an endpoint to return equality information details",
        SoThat = "it is possible to view the equality information of a person")]
    [Collection("Aws collection")]
    public class GetByIdTests : IDisposable
    {
        private readonly DynamoDbIntegrationTests<Startup> _dbFixture;
        private readonly EqualityInformationFixture _fixture;
        private readonly GetByIdSteps _steps;

        public GetByIdTests(DynamoDbIntegrationTests<Startup> dbFixture)
        {
            _dbFixture = dbFixture;
            _fixture = new EqualityInformationFixture(_dbFixture.DynamoDbContext);
            _steps = new GetByIdSteps(_dbFixture.Client);
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
        public void ServiceReturnsNotFoundIfEntityDoesntExist()
        {
            var id = Guid.NewGuid();
            var targetId = Guid.NewGuid();

            this.Given(g => _fixture.GivenAnEntityDoesNotExist())
                .When(w => _steps.WhenTheApiIsCalled(id.ToString(), targetId.ToString()))
                .Then(t => _steps.ThenNotFoundIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnBadRequestIfIdInvalid()
        {
            var id = "aaa";
            var targetId = Guid.NewGuid();

            this.Given(g => _fixture.GivenAnEntityDoesNotExist())
                .When(w => _steps.WhenTheApiIsCalled(id, targetId.ToString()))
                .Then(t => _steps.ThenBadRequestIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnBadRequestIfIdEmpty()
        {
            var id = Guid.Empty;
            var targetId = Guid.NewGuid();

            this.Given(g => _fixture.GivenAnEntityDoesNotExist())
                .When(w => _steps.WhenTheApiIsCalled(id.ToString(), targetId.ToString()))
                .Then(t => _steps.ThenBadRequestIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnBadRequestIfTargetIdInvalid()
        {
            var id = Guid.NewGuid();
            var targetId = "aaa";

            this.Given(g => _fixture.GivenAnEntityDoesNotExist())
                .When(w => _steps.WhenTheApiIsCalled(id.ToString(), targetId))
                .Then(t => _steps.ThenBadRequestIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnBadRequestIfTargetIdEmpty()
        {
            var id = Guid.NewGuid();
            var targetId = Guid.Empty;

            this.Given(g => _fixture.GivenAnEntityDoesNotExist())
                .When(w => _steps.WhenTheApiIsCalled(id.ToString(), targetId.ToString()))
                .Then(t => _steps.ThenBadRequestIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnOkWhenEntityExists()
        {
            this.Given(g => _fixture.GivenAnEntityExists())
                .When(w => _steps.WhenTheApiIsCalled(_fixture.Entity.Id.ToString(), _fixture.Entity.TargetId.ToString()))
                .Then(t => _steps.ThenTheEntityIsReturned(_fixture.Entity))
                .BDDfy();
        }
    }
}
