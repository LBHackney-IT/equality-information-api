using System;
using EqualityInformationApi.V1.Domain;
using FluentAssertions;
using Xunit;

namespace EqualityInformationApi.Tests.V1.Domain
{
    public class EntityTests
    {
        [Fact]
        public void EntitiesHaveAnId()
        {
            var entity = new Entity();
            entity.Id.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public void EntitiesHaveACreatedAt()
        {
            var entity = new Entity();
            var date = new DateTime(2019, 02, 21);
            entity.CreatedAt = date;

            entity.CreatedAt.Should().BeSameDateAs(date);
        }
    }
}
