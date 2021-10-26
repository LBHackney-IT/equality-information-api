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
    public class UpdateUseCase : IUpdateUseCase
    {
        private readonly IEqualityInformationGateway _gateway;
        public UpdateUseCase(IEqualityInformationGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<EqualityInformationResponseObject> Execute(UpdateEqualityInformationQuery query, EqualityInformationObject request, string requestBody)
        {
            var response = await _gateway.Update(query.Id, request, requestBody).ConfigureAwait(false);

            if (response == null) return null;

            return response.ToDomain().ToResponse();
        }
    }
}
