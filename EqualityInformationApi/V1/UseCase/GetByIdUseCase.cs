using System;
using System.Threading.Tasks;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Response;
using EqualityInformationApi.V1.Gateways;
using EqualityInformationApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;

namespace EqualityInformationApi.V1.UseCase
{
    public class GetByIdUseCase : IGetByIdUseCase
    {
        private IEqualityInformationGateway _gateway;
        public GetByIdUseCase(IEqualityInformationGateway gateway)
        {
            _gateway = gateway;
        }

        public Task<EqualityInformationResponseObject> Execute(GetByIdQuery query)
        {
            throw new NotImplementedException();
        }
    }
}
