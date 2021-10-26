using AutoFixture;
using EqualityInformationApi.Tests.V1.E2ETests.Fixtures;
using EqualityInformationApi.Tests.V1.E2ETests.Steps;
using EqualityInformationApi.V1.Boundary.Request;
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
    public class UpdateTests : IDisposable
    {
        private readonly DynamoDbIntegrationTests<Startup> _dbFixture;
        private readonly EqualityInformationFixture _testFixture;
        private readonly UpdateSteps _steps;
        private readonly Fixture _fixture = new Fixture();

        private const string StringWithTags = "Some string with <tag> in it.";

        public UpdateTests(DynamoDbIntegrationTests<Startup> dbFixture)
        {
            _dbFixture = dbFixture;
            _testFixture = new EqualityInformationFixture(_dbFixture.DynamoDbContext);
            _steps = new UpdateSteps(_dbFixture.Client);
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
                if (null != _testFixture)
                {
                    _testFixture.Dispose();
                }

                _disposed = true;
            }
        }

        [Fact]
        public void ServiceReturnsNotFoundWhenEntityDoesntExist()
        {
            var id = Guid.NewGuid();
            var request = _fixture.Create<EqualityInformationObject>();

            this.Given(g => _testFixture.GivenAnEntityDoesNotExist())
                .When(w => _steps.WhenTheApiIsCalled(request, id.ToString()))
                .Then(t => _steps.ThenNotFoundRequestIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsBadRequestWhenIdInvalid()
        {
            var id = Guid.Empty;

            var request = _fixture.Create<EqualityInformationObject>();

            this.Given(g => _testFixture.GivenAnEntityDoesNotExist())
                .When(w => _steps.WhenTheApiIsCalled(request, id.ToString()))
                .Then(t => _steps.ThenBadRequestIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsBadRequestWhenIdEmpty()
        {
            var id = "aaa";

            var request = _fixture.Create<EqualityInformationObject>();

            this.Given(g => _testFixture.GivenAnEntityDoesNotExist())
                .When(w => _steps.WhenTheApiIsCalled(request, id.ToString()))
                .Then(t => _steps.ThenBadRequestIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsBadRequestWhenTargetIdEmpty()
        {
            var id = Guid.NewGuid();

            var request = _fixture.Create<EqualityInformationObject>();
            request.TargetId = Guid.Empty;

            this.Given(g => _testFixture.GivenAnEntityDoesNotExist())
                .When(w => _steps.WhenTheApiIsCalled(request, id.ToString()))
                .Then(t => _steps.ThenBadRequestIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsBadRequestWhenRequestContainsTags()
        {
            var id = Guid.NewGuid();

            var request = _fixture.Create<EqualityInformationObject>();
            request.ArmedForces = StringWithTags;

            this.Given(g => _testFixture.GivenAnEntityDoesNotExist())
                .When(w => _steps.WhenTheApiIsCalled(request, id.ToString()))
                .Then(t => _steps.ThenBadRequestIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsOkWhenEntityUpdated()
        {
            var request = _fixture.Create<EqualityInformationObject>();

            this.Given(g => _testFixture.GivenAnEntityExists())
                .When(w => _steps.WhenTheApiIsCalledWithChangesInTheRequest(_testFixture.Entity))
                .Then(t => _steps.ThenTheEntityIsReturned(_testFixture._dbContext))
                .And(t => _steps.AndTheEntityWasUpdatedInTheDatabase(_testFixture._dbContext, _testFixture.Entity))
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsOkWhenNoChangesInRequest()
        {
            var request = _fixture.Create<EqualityInformationObject>();

            this.Given(g => _testFixture.GivenAnEntityExists())
                .When(w => _steps.WhenTheApiIsCalledWithNoChangesInTheRequest(_testFixture.Entity))
                .Then(t => _steps.ThenTheEntityIsReturned(_testFixture._dbContext))
                .And(t => _steps.AndTheEntityWasntUpdatedInTheDatabase(_testFixture._dbContext, _testFixture.Entity))

                .BDDfy();
        }
    }
}
