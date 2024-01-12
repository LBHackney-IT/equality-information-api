using AutoFixture;
using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Factories;
using EqualityInformationApi.V1.Infrastructure;
using EqualityInformationApi.V1.Infrastructure.Constants;
using FluentAssertions;
using Hackney.Core.DynamoDb.EntityUpdater;
using Hackney.Core.JWT;
using Hackney.Core.Sns;
using System;
using System.Collections.Generic;
using Xunit;

namespace EqualityInformationApi.Tests.V1.Factories
{
    public class SnsFactoryTests
    {
        private readonly Fixture _fixture = new Fixture();

        private UpdateEntityResult<EqualityInformationDb> CreateUpdateEntityResult(EqualityInformationDb dbEntity)
        {
            return _fixture.Build<UpdateEntityResult<EqualityInformationDb>>()
                                       .With(x => x.UpdatedEntity, dbEntity)
                                       .With(x => x.OldValues, new Dictionary<string, object> { { "title", "Dr" } })
                                       .With(x => x.NewValues, new Dictionary<string, object> { { "title", "Mr" } })
                                       .Create();
        }

        [Fact]
        public void CreateSnsEventTest()
        {
            var entity = _fixture.Create<EqualityInformation>();
            var token = _fixture.Create<Token>();

            var expectedEventData = new EventData() { NewData = entity };
            var expectedUser = new User() { Email = token.Email, Name = token.Name };

            var factory = new SnsFactory();
            var result = factory.Create(entity, token);

            result.CorrelationId.Should().NotBeEmpty();
            result.DateTime.Should().BeCloseTo(DateTime.UtcNow, 100);
            result.EntityId.Should().Be(entity.TargetId);
            result.EventData.Should().BeEquivalentTo(expectedEventData);
            result.EventType.Should().Be(CreateEventConstants.EVENTTYPE);
            result.Id.Should().NotBeEmpty();
            result.SourceDomain.Should().Be(CreateEventConstants.SOURCEDOMAIN);
            result.SourceSystem.Should().Be(CreateEventConstants.SOURCESYSTEM);
            result.User.Should().BeEquivalentTo(expectedUser);
            result.Version.Should().Be(CreateEventConstants.V1VERSION);
        }

        [Fact]
        public void UpdateSnsEventTest()
        {
            var entityDb = _fixture.Create<EqualityInformationDb>();
            var updateResult = CreateUpdateEntityResult(entityDb);
            var token = _fixture.Create<Token>();

            var expectedEventData = new EventData() { NewData = updateResult.NewValues, OldData = updateResult.OldValues };
            var expectedUser = new User() { Email = token.Email, Name = token.Name };

            var factory = new SnsFactory();
            var result = factory.Update(updateResult, token);

            result.CorrelationId.Should().NotBeEmpty();
            result.DateTime.Should().BeCloseTo(DateTime.UtcNow, 100);
            result.EntityId.Should().Be(entityDb.TargetId);
            result.EventData.Should().BeEquivalentTo(expectedEventData);
            result.EventType.Should().Be(UpdateEventConstants.EVENTTYPE);
            result.Id.Should().NotBeEmpty();
            result.SourceDomain.Should().Be(UpdateEventConstants.SOURCEDOMAIN);
            result.SourceSystem.Should().Be(UpdateEventConstants.SOURCESYSTEM);
            result.User.Should().BeEquivalentTo(expectedUser);
            result.Version.Should().Be(UpdateEventConstants.V1VERSION);
        }
    }
}
