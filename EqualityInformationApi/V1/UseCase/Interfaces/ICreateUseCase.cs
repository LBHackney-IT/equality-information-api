using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.UseCase.Interfaces
{
    public interface ICreateUseCase
    {
        Task<EqualityInformationResponseObject> Execute(EqualityInformationObject request);
    }
}
