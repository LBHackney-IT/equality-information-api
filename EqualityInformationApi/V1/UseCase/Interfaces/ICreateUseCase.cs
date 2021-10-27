using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Response;
using Hackney.Core.JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.UseCase.Interfaces
{
    public interface ICreateUseCase
    {
        Task<EqualityInformationResponseObject> Execute(EqualityInformationObject request, Token token);
    }
}
