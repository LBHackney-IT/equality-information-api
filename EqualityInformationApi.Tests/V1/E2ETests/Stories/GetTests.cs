using AutoFixture;
using EqualityInformationApi.Tests.V1.E2ETests.Fixtures;
using EqualityInformationApi.Tests.V1.E2ETests.Steps;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Domain;
using Hackney.Core.Testing.DynamoDb;
using System;
using System.Collections.Generic;
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
        private readonly Fixture _fixture = new Fixture();

        public GetTests(MockWebApplicationFactory<Startup> startupFixture)
        {
            _dbFixture = startupFixture.DynamoDbFixture;
            _testFixture = new EqualityInformationFixture(_dbFixture, startupFixture.SimpleNotificationService);
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
                if (null != _testFixture)
                {
                    _testFixture.Dispose();
                }

                _disposed = true;
            }
        }

        [Fact]
        public void ServiceGetsEntityCorrectly()
        {
            var request = _fixture.Build<PatchEqualityInformationObject>()
                .With(x => x.NationalInsuranceNumber, (string) null)
                .With(x => x.Languages, new List<LanguageInfo> { new LanguageInfo { Language = "Something", IsPrimary = true } })
                .Create();

            this.Given(x => _testFixture.GivenAnEntityExists(request.TargetId, request.Id))
                .When(w => _steps.WhenTheApiIsCalledToGet(request.TargetId))
                .Then(t => _steps.ThenTheEntityIsReturned(_dbFixture.DynamoDbContext))
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsBadRequestWhenTooManyRecords()
        {
            var request = _fixture.Build<PatchEqualityInformationObject>()
                .With(x => x.NationalInsuranceNumber, (string) null)
                .With(x => x.Languages, new List<LanguageInfo> { new LanguageInfo { Language = "Something", IsPrimary = true } })
                .Create();

            this.Given(x => _testFixture.GivenAnEntityExists(request.TargetId, request.Id))
                .And(x => _testFixture.GivenAnEntityExists(request.TargetId, Guid.NewGuid()))
                .When(w => _steps.WhenTheApiIsCalledToGet(request.TargetId))
                .Then(t => _steps.Then500IsReturned())
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsNotFoundWhenTargetIdNotFound()
        {
            var id = Guid.NewGuid();

            this.Given(g => _testFixture.GivenAnEntityDoesNotExist())
                .When(w => _steps.WhenTheApiIsCalledToGet(id))
                .Then(t => _steps.ThenNotFoundIsReturned())
                .BDDfy();
        }
    }
}
