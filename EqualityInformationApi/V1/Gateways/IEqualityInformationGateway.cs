using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.Gateways
{
    public interface IEqualityInformationGateway
    {
        Task<EqualityInformation> Create(EqualityInformationObject request);

        Task<EqualityInformation> Patch(PatchEqualityInformationObject request);
    }
}
