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
        IWant = "an endpoint to create equality information",
        SoThat = "it is possible to create equality information related to a person")]
    [Collection("Aws collection")]
    public class CreateTests : IDisposable
    {
        private readonly AWSIntegrationTests<Startup> _dbFixture;
        private readonly EqualityInformationFixture _testFixture;
        private readonly CreateSteps _steps;
        private readonly Fixture _fixture = new Fixture();

        private const string StringWithTags = "Some string with <tag> in it.";

        public CreateTests(AWSIntegrationTests<Startup> dbFixture)
        {
            _dbFixture = dbFixture;
            _testFixture = new EqualityInformationFixture(_dbFixture.DynamoDbContext, _dbFixture.SimpleNotificationService);
            _steps = new CreateSteps(_dbFixture.Client);
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
        public void ServiceReturnsBadRequestWhenTargetIdEmpty()
        {
            var request = _fixture.Create<EqualityInformationObject>();
            request.TargetId = Guid.Empty;

            this.Given(g => _testFixture.GivenAnEntityDoesNotExist())
                .When(w => _steps.WhenTheApiIsCalled(request))
                .Then(t => _steps.ThenBadRequestIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsBadRequestWhenRequestContainsTags()
        {
            var request = _fixture.Create<EqualityInformationObject>();
            request.ArmedForces = StringWithTags;

            this.Given(g => _testFixture.GivenAnEntityDoesNotExist())
                .When(w => _steps.WhenTheApiIsCalled(request))
                .Then(t => _steps.ThenBadRequestIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsCreatedWhenEntityCreated()
        {
            var request = _fixture.Create<EqualityInformationObject>();

            this.Given(g => _testFixture.GivenAnEntityDoesNotExist())
                .When(w => _steps.WhenTheApiIsCalled(request))
                .Then(t => _steps.ThenTheEntityIsReturned(_testFixture.DbContext))
                .And(t => _steps.ThenTheEqualityInformationCreatedEventIsRaised(_testFixture, _dbFixture.SnsVerifer))
                .BDDfy();
        }
    }
}
