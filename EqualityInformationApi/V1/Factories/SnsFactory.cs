using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Infrastructure;
using EqualityInformationApi.V1.Infrastructure.Constants;
using Hackney.Core.DynamoDb.EntityUpdater;
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
                EntityId = equalityInformation.TargetId,
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

        public EntityEventSns Update(UpdateEntityResult<EqualityInformationDb> updateResult, Token token)
        {
            return new EntityEventSns
            {
                CorrelationId = Guid.NewGuid(),
                DateTime = DateTime.UtcNow,
                EntityId = updateResult.UpdatedEntity.TargetId,
                Id = Guid.NewGuid(),
                EventType = UpdateEventConstants.EVENTTYPE,
                Version = UpdateEventConstants.V1VERSION,
                SourceDomain = UpdateEventConstants.SOURCEDOMAIN,
                SourceSystem = UpdateEventConstants.SOURCESYSTEM,
                User = new User
                {
                    Name = token.Name,
                    Email = token.Email
                },
                EventData = new EventData
                {
                    NewData = updateResult.NewValues,
                    OldData = updateResult.OldValues
                }
            };
        }
    }
}
