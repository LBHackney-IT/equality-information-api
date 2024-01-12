using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Infrastructure;
using Hackney.Core.DynamoDb.EntityUpdater;
using System;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.Gateways
{
    public interface IEqualityInformationGateway
    {
        Task<EqualityInformation> Create(EqualityInformationObject request);

        Task<UpdateEntityResult<EqualityInformationDb>> Update(PatchEqualityInformationRequest request,
            EqualityInformationObject requestObject, string bodyText, int? ifMatch);

        Task<EqualityInformation> Get(Guid targetId);
    }
}
