using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Domain;
using System;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.Gateways
{
    public interface IEqualityInformationGateway
    {
        Task<EqualityInformation> Create(EqualityInformationObject request);

        Task<EqualityInformation> Update(PatchEqualityInformationObject request);

        Task<EqualityInformation> Get(Guid targetId);
    }
}
