using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Infrastructure.Constants;
using Hackney.Core.JWT;
using Hackney.Core.Sns;
using System;

namespace EqualityInformationApi.V1.Factories
{
    public class SnsFactory : ISnsFactory
    {
        public EntityEventSns Create(EqualityInformation equalityInformation, Token token)
        {
            return new EntityEventSns
            {
                CorrelationId = Guid.NewGuid(),
                DateTime = DateTime.UtcNow,
                EntityId = equalityInformation.Id,
                Id = Guid.NewGuid(),
                EventType = CreateEventConstants.EVENTTYPE,
                Version = CreateEventConstants.V1VERSION,
                SourceDomain = CreateEventConstants.SOURCEDOMAIN,
                SourceSystem = CreateEventConstants.SOURCESYSTEM,
                User = new User
                {
                    Name = token.Name,
                    Email = token.Email
                },
                EventData = new EventData
                {
                    NewData = equalityInformation
                }
            };
        }
    }
}
