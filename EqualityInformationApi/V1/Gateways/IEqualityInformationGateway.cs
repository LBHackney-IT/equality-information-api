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
        Task<List<EqualityInformationDb>> GetAll(Guid targetId);
        Task<EqualityInformationDb> GetById(Guid id, Guid targetId);
        Task<EqualityInformationDb> Create(EqualityInformationObject request);
        Task<EqualityInformationDb> Update(Guid id, EqualityInformationObject request, string requestObject);
    }
}
