using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Gateways;
using EqualityInformationApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;
using System;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.UseCase
{
    public class GetUseCase : IGetUseCase
    {
        private readonly IEqualityInformationGateway _gateway;

        public GetUseCase(IEqualityInformationGateway gateway)
        {
            _gateway = gateway;
        }

        [LogCall]
        public async Task<EqualityInformation> Execute(Guid targetId)
        {
            return await _gateway.Get(targetId).ConfigureAwait(false);
        }
    }
}
