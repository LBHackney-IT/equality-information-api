using AutoFixture;
using EqualityInformationApi.Tests.V1.E2ETests.Fixtures;
using EqualityInformationApi.Tests.V1.E2ETests.Steps;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Request.Validation;
using EqualityInformationApi.V1.Domain;
using Hackney.Core.Sns;
using Hackney.Core.Testing.DynamoDb;
using System;
using System.Collections.Generic;
using TestStack.BDDfy;
using Xunit;

namespace EqualityInformationApi.Tests.V1.E2ETests.Stories
{
    [Story(
        AsA = "Service",
        IWant = "an endpoint to patch equality information",
        SoThat = "it is possible to patch equality information related to a person")]
    [Collection("Aws collection")]
    public class PatchTests : IDisposable
    {
        private readonly IDynamoDbFixture _dbFixture;
        private readonly SnsEventVerifier<EntityEventSns> _snsVerifier;
        private readonly EqualityInformationFixture _testFixture;
        private readonly PatchSteps _steps;
        private readonly Fixture _fixture = new Fixture();

        public PatchTests(MockWebApplicationFactory<Startup> startupFixture)
        {
            _dbFixture = startupFixture.DynamoDbFixture;
            _snsVerifier = startupFixture.SnsVerifer;
            _testFixture = new EqualityInformationFixture(_dbFixture, startupFixture.SimpleNotificationService);
            _steps = new PatchSteps(startupFixture.Client);
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
        public void ServicePatchesEntityCorrectly()
        {
            var id = Guid.NewGuid();
            var requestObject = new EqualityInformationObject()
            {
                TargetId = Guid.NewGuid(),
                NationalInsuranceNumber = "NZ123456D",
                Languages = new List<LanguageInfo> { new LanguageInfo { Language = "Something", IsPrimary = true } }
            };

            this.Given(x => _testFixture.GivenAnEntityExists(requestObject.TargetId, id))
                .When(w => _steps.WhenTheApiIsCalledToPatch(requestObject, id))
                .Then(t => _steps.ThenTheEntityIsReturned(_dbFixture.DynamoDbContext))
                .And(t => _steps.ThenTheUpdatedSnsEventIsRaised(_testFixture, _snsVerifier))
                .BDDfy();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(5)]
        public void PatchServiceReturnsConflictWhenIncorrectVersionNumber(int? versionNumber)
        {
            var id = Guid.NewGuid();
            var requestObject = _fixture.Build<EqualityInformationObject>()
                .With(x => x.NationalInsuranceNumber, (string) null)
                .With(x => x.Languages, new List<LanguageInfo> { new LanguageInfo { Language = "Something", IsPrimary = true } })
                .Create();

            this.Given(x => _testFixture.GivenAnEntityExists(requestObject.TargetId, id))
                .When(w => _steps.WhenTheApiIsCalledToPatch(requestObject, id, versionNumber))
                .Then(t => _steps.ThenConflictIsReturned(versionNumber))
                .BDDfy();
        }

        [Fact]
        public void PatchServiceReturnsNotFoundtWhenTargetIdNotFound()
        {
            var id = Guid.NewGuid();
            var requestObject = _fixture.Build<EqualityInformationObject>()
                .With(x => x.NationalInsuranceNumber, (string) null)
                .With(x => x.Languages, new List<LanguageInfo> { new LanguageInfo { Language = "Something", IsPrimary = true } })
                .Create();

            this.Given(g => _testFixture.GivenAnEntityDoesNotExist())
                .When(w => _steps.WhenTheApiIsCalledToPatch(requestObject, id))
                .Then(t => _steps.ThenNotFoundIsReturned())
                .BDDfy();
        }

        [Fact]
        public void PatchServiceReturnsBadRequestWhenRequestInvalid()
        {
            var id = Guid.NewGuid();
            var requestObject = _fixture.Build<EqualityInformationObject>()
                .With(x => x.NationalInsuranceNumber, "dgfdsfdsf")
                .With(x => x.Languages, new List<LanguageInfo> { new LanguageInfo { Language = "Something", IsPrimary = false } })
                .Create();

            this.Given(g => _testFixture.GivenAnEntityDoesNotExist())
                .When(w => _steps.WhenTheApiIsCalledToPatch(requestObject, id))
                .Then(t => _steps.ThenBadRequestIsReturned())
                .Then(t => _steps.ThenTheValidationErrorsAreReturned("Languages", ErrorCodes.OnePrimaryLanguage))
                .Then(t => _steps.ThenTheValidationErrorsAreReturned("NationalInsuranceNumber"))
                .BDDfy();
        }

        [Fact]
        public void PatchServiceReturnsBadRequestWhenTargetIdEmpty()
        {
            var id = Guid.NewGuid();
            var requestObject = _fixture.Build<EqualityInformationObject>()
                .With(x => x.TargetId, Guid.Empty)
                .Create();

            this.Given(g => _testFixture.GivenAnEntityDoesNotExist())
                .When(w => _steps.WhenTheApiIsCalledToPatch(requestObject, id))
                .Then(t => _steps.ThenBadRequestIsReturned())
                .BDDfy();
        }
    }
}
