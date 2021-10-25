using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Response;
using EqualityInformationApi.V1.Factories;
using EqualityInformationApi.V1.Gateways;
using EqualityInformationApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.UseCase
{
    public class GetAllUseCase : IGetAllUseCase
    {
        private readonly IEqualityInformationGateway _gateway;
        public GetAllUseCase(IEqualityInformationGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<GetAllResponseObject> Execute(EqualityInformationQuery query)
        {
            var response = await _gateway.GetAll(query.TargetId).ConfigureAwait(false);

            return response.ToDomain().ToResponse();
        }
    }
}
