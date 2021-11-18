using System;
using System.Threading.Tasks;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Response;
using EqualityInformationApi.V1.Factories;
using EqualityInformationApi.V1.Gateways;
using EqualityInformationApi.V1.UseCase.Interfaces;
using Hackney.Core.JWT;
using Hackney.Core.Sns;

namespace EqualityInformationApi.V1.UseCase
{
    public class PatchUseCase : IPatchUseCase
    {
        private readonly IEqualityInformationGateway _gateway;
        private readonly ISnsGateway _snsGateway;
        private readonly ISnsFactory _snsFactory;

        public PatchUseCase(
            IEqualityInformationGateway gateway,
            ISnsGateway snsGateway,
            ISnsFactory snsFactory)
        {
            _gateway = gateway;
            _snsGateway = snsGateway;
            _snsFactory = snsFactory;
        }

        public async Task<EqualityInformationResponseObject> Execute(PatchEqualityInformationObject request, Token token, int? ifMatch)
        {
            var equalityInformation = await _gateway.Update(request, ifMatch).ConfigureAwait(false);
            if (equalityInformation is null) return null;

            var createSnsMessage = _snsFactory.Update(equalityInformation, token);
            var tenureTopicArn = Environment.GetEnvironmentVariable("EQUALITY_INFORMATION_SNS_ARN");

            await _snsGateway.Publish(createSnsMessage, tenureTopicArn).ConfigureAwait(false);

            return equalityInformation.ToResponse();
        }
    }
}
