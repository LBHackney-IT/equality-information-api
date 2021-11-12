using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Response;
using EqualityInformationApi.V1.Factories;
using EqualityInformationApi.V1.Gateways;
using EqualityInformationApi.V1.UseCase.Interfaces;
using Hackney.Core.JWT;
using Hackney.Core.Sns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.UseCase
{
    public class GetUseCase : IGetUseCase
    {
        private readonly IEqualityInformationGateway _gateway;

        public GetUseCase(
            IEqualityInformationGateway gateway,
            ISnsGateway snsGateway,
            ISnsFactory snsFactory)
        {
            _gateway = gateway;
        }

        public async Task<List<EqualityInformationResponseObject>> Execute(Guid targetId, Token token)
        {
            var equalityInformation = await _gateway.Get(targetId).ConfigureAwait(false);

            return equalityInformation.Select(x => x.ToResponse()).ToList();
        }
    }
}
