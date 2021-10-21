using System;
using EqualityInformationApi.V1.Boundary.Response;
using EqualityInformationApi.V1.Gateways;
using EqualityInformationApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;

namespace EqualityInformationApi.V1.UseCase
{
    //TODO: Rename class name and interface name to reflect the entity they are representing eg. GetClaimantByIdUseCase
    public class GetByIdUseCase : IGetByIdUseCase
    {
        private IExampleDynamoGateway _gateway;
        public GetByIdUseCase(IExampleDynamoGateway gateway)
        {
            _gateway = gateway;
        }
        [LogCall]
        //TODO: rename id to the name of the identifier that will be used for this API, the type may also need to change
        public ResponseObject Execute(int id)
        {
            // return _gateway.GetEntityById(id).ToResponse();
            throw new NotImplementedException();
        }
    }
}
