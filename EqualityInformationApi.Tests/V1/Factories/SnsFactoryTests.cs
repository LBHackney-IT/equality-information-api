using AutoFixture;
using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Factories;
using EqualityInformationApi.V1.Infrastructure.Constants;
using FluentAssertions;
using Hackney.Core.JWT;
using Hackney.Core.Sns;
using System;
using Xunit;

namespace EqualityInformationApi.Tests.V1.Factories
{
    public class SnsFactoryTests
    {
        private readonly Fixture _fixture = new Fixture();

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
            result.EntityId.Should().Be(entity.Id);
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
            var entity = _fixture.Create<EqualityInformation>();
            var token = _fixture.Create<Token>();

            var expectedEventData = new EventData() { NewData = entity };
            var expectedUser = new User() { Email = token.Email, Name = token.Name };

            var factory = new SnsFactory();
            var result = factory.Update(entity, token);

            result.CorrelationId.Should().NotBeEmpty();
            result.DateTime.Should().BeCloseTo(DateTime.UtcNow, 100);
            result.EntityId.Should().Be(entity.Id);
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
