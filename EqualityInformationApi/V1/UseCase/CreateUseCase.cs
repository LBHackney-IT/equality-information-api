using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Response;
using EqualityInformationApi.V1.Gateways;
using EqualityInformationApi.V1.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.UseCase
{
    public class CreateUseCase : ICreateUseCase
    {
        private readonly IEqualityInformationGateway _gateway;
        public CreateUseCase(IEqualityInformationGateway gateway)
        {
            _gateway = gateway;
        }

        public Task<EqualityInformationResponseObject> Execute(EqualityInformationObject request)
        {
            throw new NotImplementedException();
        }
    }
}
