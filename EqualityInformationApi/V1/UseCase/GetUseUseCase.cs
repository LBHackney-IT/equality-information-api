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
    public class GetUseUseCase : IGetUseCase
    {
        private readonly IEqualityInformationGateway _gateway;
        private readonly ISnsGateway _snsGateway;
        private readonly ISnsFactory _snsFactory;

        public GetUseUseCase(
            IEqualityInformationGateway gateway,
            ISnsGateway snsGateway,
            ISnsFactory snsFactory)
        {
            _gateway = gateway;
            _snsGateway = snsGateway;
            _snsFactory = snsFactory;
        }

        public async Task<EqualityInformationResponseObject> Execute(string targetId, Token token)
        {
            var equalityInformation = await _gateway.Get(targetId).ConfigureAwait(false);

            var createSnsMessage = _snsFactory.Create(equalityInformation, token);
            var tenureTopicArn = Environment.GetEnvironmentVariable("EQUALITY_INFORMATION_SNS_ARN");

            await _snsGateway.Publish(createSnsMessage, tenureTopicArn).ConfigureAwait(false);

            return equalityInformation.ToResponse();
        }
    }
}
