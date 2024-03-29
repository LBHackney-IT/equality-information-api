using AutoFixture;
using EqualityInformationApi.Tests.V1.E2ETests.Fixtures;
using EqualityInformationApi.Tests.V1.E2ETests.Steps;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Request.Validation;
using EqualityInformationApi.V1.Domain;
using Hackney.Core.Testing.DynamoDb;
using Hackney.Core.Testing.Sns;
using System;
using System.Collections.Generic;
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
        private readonly IDynamoDbFixture _dbFixture;
        private readonly ISnsFixture _snsFixture;
        private readonly EqualityInformationFixture _testFixture;
        private readonly CreateSteps _steps;
        private readonly Fixture _fixture = new Fixture();

        public CreateTests(MockWebApplicationFactory<Startup> startupFixture)
        {
            _dbFixture = startupFixture.DynamoDbFixture;
            _snsFixture = startupFixture.SnsFixture;
            _testFixture = new EqualityInformationFixture(_dbFixture, _snsFixture.SimpleNotificationService);
            _steps = new CreateSteps(startupFixture.Client);
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
                _snsFixture?.PurgeAllQueueMessages();

                _disposed = true;
            }
        }

        [Fact]
        public void ServiceReturnsBadRequestWhenTargetIdEmpty()
        {
            var request = _fixture.Create<EqualityInformationObject>();
            request.TargetId = Guid.Empty;

            this.Given(g => _testFixture.GivenAnEntityDoesNotExist())
                .When(w => _steps.WhenTheApiIsCalledToCreate(request))
                .Then(t => _steps.ThenBadRequestIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsBadRequestWhenRequestFailsValidation()
        {
            var request = _fixture.Build<EqualityInformationObject>()
                                  .With(x => x.ArmedForces, "Some string with <tag> in it.")
                                  .With(x => x.NationalInsuranceNumber, "InvalidNI")
                                  .With(x => x.Languages, new List<LanguageInfo> { new LanguageInfo { Language = "Something", IsPrimary = false } })
                                  .Create();
            var errorInfo = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("ArmedForces", ErrorCodes.XssCheckFailure),
                new KeyValuePair<string, string>("NationalInsuranceNumber", null),
                new KeyValuePair<string, string>("Languages", ErrorCodes.OnePrimaryLanguage),
            };

            this.Given(g => _testFixture.GivenAnEntityDoesNotExist())
                .When(w => _steps.WhenTheApiIsCalledToCreate(request))
                .Then(t => _steps.ThenBadRequestIsReturned())
                .Then(t => _steps.ThenTheValidationErrorsAreReturned(errorInfo))
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsCreatedWhenEntityCreated()
        {
            var request = _fixture.Build<EqualityInformationObject>()
                                  .With(x => x.NationalInsuranceNumber, (string) null)
                                  .With(x => x.Languages, new List<LanguageInfo> { new LanguageInfo { Language = "Something", IsPrimary = true } })
                                  .Create();

            this.Given(g => _testFixture.GivenAnEntityDoesNotExist())
                .When(w => _steps.WhenTheApiIsCalledToCreate(request))
                .Then(t => _steps.ThenTheEntityIsReturned(_dbFixture.DynamoDbContext))
                .And(t => _steps.ThenTheEqualityInformationCreatedEventIsRaised(_testFixture, _snsFixture))
                .BDDfy();
        }
    }
}
