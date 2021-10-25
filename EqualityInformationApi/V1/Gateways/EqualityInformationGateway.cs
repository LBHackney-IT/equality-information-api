using Amazon.DynamoDBv2.DataModel;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Factories;
using EqualityInformationApi.V1.Infrastructure;
using Hackney.Core.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.Gateways
{
    public class EqualityInformationGateway : IEqualityInformationGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly ILogger<EqualityInformationGateway> _logger;


        public EqualityInformationGateway(IDynamoDBContext dynamoDbContext, ILogger<EqualityInformationGateway> logger)
        {
            _dynamoDbContext = dynamoDbContext;
            _logger = logger;
        }

        public Task<EqualityInformationDb> Create(EqualityInformationObject request)
        {
            throw new NotImplementedException();
        }

        public Task<List<EqualityInformationDb>> GetAll(Guid targetId)
        {
            throw new NotImplementedException();
        }

        public Task<EqualityInformationDb> GetById(Guid id, Guid targetId)
        {
            throw new NotImplementedException();
        }

        public Task<EqualityInformationDb> Update(Guid id, EqualityInformationObject request)
        {
            throw new NotImplementedException();
        }
    }
}
