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
        private readonly IEntityUpdater _updater;

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
        public async Task<EqualityInformationDb> Create(EqualityInformationObject request)
        {
            var entity = request.ToDomain().ToDatabase();

            await SaveEntity(entity).ConfigureAwait(false);

            return entity;
        }

        [LogCall]
        public async Task<List<EqualityInformationDb>> GetAll(Guid targetId)
        {
            var directoryList = new List<EqualityInformationDb>();

            _logger.LogDebug($"Calling IDynamoDBContext.QueryAsync for {targetId}");

            var search = _dynamoDbContext.QueryAsync<EqualityInformationDb>(targetId);

            do
            {
                var newDirectorys = await search.GetNextSetAsync().ConfigureAwait(false);
                directoryList.AddRange(newDirectorys);

            } while (search.IsDone == false);

            return directoryList;
        }

        [LogCall]
        public async Task<EqualityInformationDb> GetById(Guid id, Guid targetId)
        {
            return await LoadEntity(targetId, id).ConfigureAwait(false);
        }

        [LogCall]
        public async Task<EqualityInformationDb> Update(Guid id, EqualityInformationObject request, string requestBody)
        {
            var existingEntity = await LoadEntity(request.TargetId, id).ConfigureAwait(false);
            if (existingEntity == null) return null;

            var result = _updater.UpdateEntity(existingEntity, requestBody, request);
            if (result.NewValues.Any())
            {
                await SaveEntity(result.UpdatedEntity).ConfigureAwait(false);
            }

            return result.UpdatedEntity;
        }

        private async Task SaveEntity(EqualityInformationDb entity)
        {
            _logger.LogDebug($"Calling IDynamoDBContext.SaveAsync for {entity.TargetId}.{entity.Id}");

            await _dynamoDbContext.SaveAsync<EqualityInformationDb>(entity).ConfigureAwait(false);
        }

        private async Task<EqualityInformationDb> LoadEntity(Guid targetId, Guid id)
        {
            _logger.LogDebug($"Calling IDynamoDBContext.LoadAsync for id {targetId}.{id}");

            return await _dynamoDbContext.LoadAsync<EqualityInformationDb>(targetId, id).ConfigureAwait(false);
        }
    }
}
