using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.UseCase.Interfaces
{
    public interface IUpdateUseCase
    {
        Task<EqualityInformationResponseObject> Execute(UpdateQualityInformationQuery query, EqualityInformationObject request, string requestBody);
    }
}
