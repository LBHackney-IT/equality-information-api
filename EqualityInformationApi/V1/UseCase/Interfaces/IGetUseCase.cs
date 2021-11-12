using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EqualityInformationApi.V1.Boundary.Response;
using Hackney.Core.JWT;

namespace EqualityInformationApi.V1.UseCase.Interfaces
{
    public interface IGetUseCase
    {
        Task<List<EqualityInformationResponseObject>> Execute(Guid targetId, Token token);
    }
}
