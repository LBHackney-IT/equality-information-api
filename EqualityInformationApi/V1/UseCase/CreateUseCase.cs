using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Response;
using EqualityInformationApi.V1.Factories;
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

        public async Task<EqualityInformationResponseObject> Execute(EqualityInformationObject request)
        {
            var response = await _gateway.Create(request).ConfigureAwait(false);

            if (response == null) return null;

            return response.ToDomain().ToResponse();
        }
    }
}
