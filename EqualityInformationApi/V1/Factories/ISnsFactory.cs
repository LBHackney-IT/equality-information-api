using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Infrastructure;
using Hackney.Core.JWT;
using Hackney.Core.Sns;

namespace EqualityInformationApi.V1.Factories
{
    public interface ISnsFactory
    {
        EntityEventSns Create(EqualityInformation equalityInformation, Token token);
        EntityEventSns Update(UpdateEntityResult<EqualityInformationDb> updateResult, Token token);
    }
}
