using System;
using System.Threading.Tasks;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Response;
using EqualityInformationApi.V1.Factories;
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

        public async Task<EqualityInformationResponseObject> Execute(GetByIdQuery query)
        {
            var response = await _gateway.GetById(query.Id, query.TargetId).ConfigureAwait(false);

            if (response == null) return null;

            return response.ToDomain().ToResponse();
        }
    }
}
