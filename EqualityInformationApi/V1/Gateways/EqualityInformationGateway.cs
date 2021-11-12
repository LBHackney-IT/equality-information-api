using Amazon.DynamoDBv2.DataModel;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Factories;
using EqualityInformationApi.V1.Infrastructure;
using Hackney.Core.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.Gateways
{
    public class EqualityInformationGateway : IEqualityInformationGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly ILogger<EqualityInformationGateway> _logger;

        public EqualityInformationGateway(
            IDynamoDBContext dynamoDbContext,
            ILogger<EqualityInformationGateway> logger)
        {
            _dynamoDbContext = dynamoDbContext;
            _logger = logger;
        }

        [LogCall]
        public async Task<EqualityInformation> Create(EqualityInformationObject request)
        {
            var entity = request.ToDomain().ToDatabase();

            _logger.LogDebug($"Calling IDynamoDBContext.SaveAsync for {entity.TargetId}.{entity.Id}");

            await _dynamoDbContext.SaveAsync(entity).ConfigureAwait(false);

            return entity.ToDomain();
        }

        public async Task<EqualityInformation> Update(PatchEqualityInformationObject request)
        {
            var entity = request.ToDomain().ToDatabase();

            _logger.LogDebug($"Calling IDynamoDBContext.SaveAsync for {entity.TargetId}.{entity.Id}");

            await _dynamoDbContext.SaveAsync(entity).ConfigureAwait(false);

            return entity.ToDomain();
        }
    }
}
