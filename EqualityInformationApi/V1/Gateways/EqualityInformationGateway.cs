using Amazon.DynamoDBv2.DataModel;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Factories;
using EqualityInformationApi.V1.Infrastructure;
using EqualityInformationApi.V1.Infrastructure.Exceptions;
using Hackney.Core.DynamoDb.EntityUpdater;
using Hackney.Core.DynamoDb.EntityUpdater.Interfaces;
using Hackney.Core.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.Gateways
{
    public class EqualityInformationGateway : IEqualityInformationGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly IEntityUpdater _updater;
        private readonly ILogger<EqualityInformationGateway> _logger;

        public EqualityInformationGateway(
            IDynamoDBContext dynamoDbContext,
            IEntityUpdater updater,
            ILogger<EqualityInformationGateway> logger)
        {
            _dynamoDbContext = dynamoDbContext;
            _updater = updater;
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

        [LogCall]
        public async Task<UpdateEntityResult<EqualityInformationDb>> Update(PatchEqualityInformationRequest request,
            EqualityInformationObject requestObject, string bodyText, int? ifMatch)
        {
            var existingRecord = await _dynamoDbContext.LoadAsync<EqualityInformationDb>(requestObject.TargetId, request.Id)
                                                       .ConfigureAwait(false);
            if (existingRecord == null) return null;

            if (ifMatch != existingRecord.VersionNumber)
                throw new VersionNumberConflictException(ifMatch, existingRecord.VersionNumber);

            var response = _updater.UpdateEntity(existingRecord, bodyText, requestObject);
            if (response.NewValues.Any())
            {
                _logger.LogDebug($"Calling IDynamoDBContext.SaveAsync for targetId: {requestObject.TargetId}; id: {request.Id}");
                await _dynamoDbContext.SaveAsync(response.UpdatedEntity).ConfigureAwait(false);
            }

            return response;
        }

        [LogCall]
        public async Task<EqualityInformation> Get(Guid targetId)
        {
            _logger.LogDebug($"Calling IDynamoDBContext.QueryAsync for target id {targetId}. Expecting 0 or 1 result.");

            var results = await _dynamoDbContext.QueryAsync<EqualityInformationDb>(targetId)
                                                .GetNextSetAsync()
                                                .ConfigureAwait(false);

            if ((results is null) || !results.Any())
                return null;

            if (results.Count > 1)
                throw new ApplicationException($"{results.Count} EqualityInformationDb records found for target id {targetId}. "
                                              + "There should only be 0 or 1.");

            return results.First().ToDomain();
        }
    }
}
